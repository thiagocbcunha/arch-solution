using System.Diagnostics;
using Verx.Enterprise.Tracing;
using Verx.Enterprise.Correlation;
using System.Diagnostics.CodeAnalysis;
using OpenTelemetry.Context.Propagation;

namespace Verx.Enterprise.MessageBroker;

[ExcludeFromCodeCoverage]
public class InternalTraceFacade(ITracer? tracer, ApplicationCorrelation correlation)
{
    private readonly ITracer? _tracer = tracer;
    private readonly ApplicationCorrelation _correlation = correlation;

    internal ISpan CreateConsumerSpan<TClass, TMessage>(PropagationContext propagationContext, Guid correlation, string system)
        where TClass : class
        where TMessage : class
    {
        var span = _tracer?.RootSpan<TClass>(ActivityKind.Consumer, propagationContext.ActivityContext) ?? new NullSpan();

        _correlation.SetCorrelation(correlation);
        SetBasicKeys<TMessage>(span, system);

        return span;
    }

    internal ISpan CreateProducerSpan<TClass, TMessage>(string system)
        where TClass : class
        where TMessage : class
    {
        var span = _tracer?.RootSpan<TClass>(ActivityKind.Producer) ?? new NullSpan();
        SetBasicKeys<TMessage>(span, system);

        return span;
    }

    private void SetBasicKeys<TMessage>(ISpan span, string system)
    {
        span.SetKey("message.correlationid", _correlation.Id);
        span.SetKey("message.type", typeof(TMessage).Name);
        span.SetKey("message.operation", "processing");
        span.SetKey("message.system", system);
    }
}
