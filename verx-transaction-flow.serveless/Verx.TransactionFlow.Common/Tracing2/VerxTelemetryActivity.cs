using System.Diagnostics;
using Microsoft.Extensions.Options;
using Verx.TransactionFlow.Common.Options;
using Verx.TransactionFlow.Common.Contracts;

namespace Verx.TransactionFlow.Common.Tracing;

public class VerxTelemetryActivity(IOptions<ObservabilitySettings> options) : IActivity, ITelemetryActivity
{
    private readonly ObservabilitySettings _config = options.Value;

    private Activity? _activity;

    public IActivity Start<TCaller>()
    {
        return Start(typeof(TCaller).Name);
    }

    public IActivity Start(string identify)
    {
        var activitySource = new ActivitySource(_config.Name, _config.Version);
        _activity = activitySource.StartActivity(identify);

        return this;
    }

    public void Dispose()
    {
        _activity?.Dispose();
        _activity = null;
    }

    public IActivity SetTag(string key, object value)
    {
        if (_activity is null)
            throw new InvalidOperationException("Activity has not been started.");

        _activity.SetTag(key, value);
        
        return this;
    }
}