using MassTransit;

namespace Verx.TransactionFlow.Infrastructure.MessageBrokers.RabbitMQ;

public class LoggingObserver : IReceiveObserver
{
    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
    {
        Console.WriteLine($"Mensagem consumida por: {consumerType}");
        return Task.CompletedTask;
    }

    public Task PreReceive(ReceiveContext context) => Task.CompletedTask;
    public Task PostReceive(ReceiveContext context) => Task.CompletedTask;
    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception) where T : class => Task.CompletedTask;
    public Task ReceiveFault(ReceiveContext context, Exception exception) => Task.CompletedTask;
}
