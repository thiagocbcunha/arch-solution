using System.Text.Json;
using Verx.Enterprise.Tracing;
using System.Threading.Channels;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Verx.Enterprise.MessageBroker.RabbitMQ;

namespace Verx.Enterprise.MessageBroker;

[ExcludeFromCodeCoverage]
public class BrokerChannel<TMessage>(Channel<EventPackage<TMessage>> channel, ITracer tracer, IRabbitProducer<TMessage> rabbitProducer) : IChannel<TMessage>
    where TMessage : class, new()
{
    private EventPackage<TMessage>? _packageInProcess;

    public async IAsyncEnumerable<TMessage?> ConsumeAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var package in channel.Reader.ReadAllAsync(cancellationToken))
        {
            _packageInProcess = package;

            tracer.AddParentContext(package.ParentContext);

            if (package.IsKafka)
            {
                yield return JsonSerializer.Deserialize<TMessage>(package.KafkaMessage?.Message?.Value ?? "");
                continue;
            }

            yield return JsonSerializer.Deserialize<TMessage>(package.RabbitMessage?.Body.ToArray());
        }
    }

    public async Task RaiserError(ISpan span)
    {
        span.Failure("Message processing failed. Attempting to handle failure.");

        if (_packageInProcess?.IsRabbit ?? false)
            await rabbitProducer.PublishErroEventAsynt(span, _packageInProcess.RabbitMessage!);

        else if (_packageInProcess?.IsKafka ?? false)
        {
            if (!string.IsNullOrEmpty(_packageInProcess.KafkaMessage?.Message?.Value) && 
                JsonSerializer.Deserialize<TMessage>(_packageInProcess.KafkaMessage.Message.Value) is TMessage message)
                await rabbitProducer.PublishAsync(message);
        }
    }
}
