namespace Verx.TransactionFlow.Common.Contracts;

public interface ITelemetryActivity
{
    IActivity Start<TCaller>();
    IActivity Start(string identify);
}