namespace Verx.TransactionFlow.Common.Contracts;

public interface IEnterpriseSecurity
{
    string GetHash(string value);
}