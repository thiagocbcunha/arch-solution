using System.Text;
using OpenTelemetry;
using RabbitMQ.Client;
using System.Text.Json;
using System.Reflection;
using RabbitMQ.Client.Events;
using Verx.Enterprise.Tracing;
using Verx.Enterprise.Correlation;
using OpenTelemetry.Context.Propagation;
using Verx.Enterprise.MessageBroker.RabbitMQ.Extensions;

namespace Verx.Enterprise.MessageBroker.RabbitMQ.Client;

public class RabbitProducer<TMessage>(IConnection _connection, InternalTraceFacade _tracer, ICorrelation _correlation) : IRabbitProducer<TMessage>
    where TMessage : class, new()
{
    private bool _isConfigured = false;
    private readonly IModel _model = _connection.CreateModel();
    private readonly Topology _topology = new (typeof(TMessage));

    public Task PublishAsync(TMessage mensagem, CancellationToken cancellationToken = default)
    {
        if (!_isConfigured)
        {
            _isConfigured = true;
            _model.ConfigureTopoligy(_topology);
        }

        using var activity = _tracer.CreateProducerSpan<RabbitProducer<TMessage>, TMessage>("rabbitmq");

        var props = _model.CreateBasicProperties();
        props.Persistent = true;
        props.Headers = new Dictionary<string, object>
        {
            [Correlation.Constants.CorrelationIdHeader] = Encoding.UTF8.GetBytes(_correlation.Id.ToString())
        };

        Propagators.DefaultTextMapPropagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectHeader);

        var body = JsonSerializer.SerializeToUtf8Bytes(mensagem);
        _model.BasicPublish(_topology.Exchange, _topology.Queue, props, body);

        return Task.CompletedTask;
    }

    Task IRabbitProducer<TMessage>.PublishErroEventAsynt(ISpan span, BasicDeliverEventArgs eventMessage)
    {
        var retryPolicy = typeof(TMessage).GetCustomAttribute<RetryPolicyAttribute>(true);

        var retryCountFactor = 0;
        var sentinel = retryPolicy?.MaxRetry ?? 3;
        var milleSecondsDelay = retryPolicy?.Delay ?? 15 * 1000;
        var retryCount = eventMessage.BasicProperties.GetRetryCount();

        span.SetKey("RetryCount", retryCount);
        span.Failure($"Message processing failed. Attempting to handle failure. Retry count: {retryCount}");
        if (retryPolicy?.Exponential ?? true)
            retryCountFactor = retryCount;

        if (retryCount >= sentinel)
            _model.BasicPublish(_topology.DlqExchange, _topology.DlqQueue, eventMessage.BasicProperties, eventMessage.Body);

        else
        {
            var delay = (retryCountFactor + 1).CalculateDalay(milleSecondsDelay);

            var props = _model.CreateBasicProperties();
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>
            {
                { "x-delay", delay },
                { "x-retry-count", Encoding.UTF8.GetBytes((retryCount + 1).ToString()) }
            };

            var context = new PropagationContext(span?.Context ?? default, Baggage.Current);

            Propagators.DefaultTextMapPropagator.Inject(context, props, InjectHeader);

            _model.BasicPublish(_topology.RetryExchange, _topology.RetryQueue, props, eventMessage.Body);
        }

        return Task.CompletedTask;
    }

    private static void InjectHeader(IBasicProperties basicProperties, string key, string value)
    {
        basicProperties.Headers.Add(key, Encoding.UTF8.GetBytes(value));
    }
}
