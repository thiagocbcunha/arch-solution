namespace Verx.Consolidated.Domain.Contracts;

public interface IMessagingSender
{
    Task Send<TMessage>(TMessage message) where TMessage : class;
}