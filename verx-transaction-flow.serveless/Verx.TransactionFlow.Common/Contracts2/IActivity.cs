namespace Verx.TransactionFlow.Common.Contracts;

public interface IActivity
{
    IActivity SetTag(string key, object value);
}
