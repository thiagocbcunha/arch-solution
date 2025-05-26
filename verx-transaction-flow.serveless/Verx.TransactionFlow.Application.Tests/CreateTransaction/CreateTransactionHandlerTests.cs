using Moq;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Verx.TransactionFlow.Domain.Event;
using Verx.Enterprise.MessageBroker.Kafka;
using Verx.TransactionFlow.Application.CreateTransation;

namespace Verx.TransactionFlow.Application.Tests.CreateTransaction;

public class CreateTransactionHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPublishEventAndReturnSuccess()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateTransactionHandler>>();
        var tracerMock = new Mock<ITracer>();
        var spanMock = new Mock<ISpan>();
        tracerMock.Setup(t => t.Span<CreateTransactionHandler>()).Returns(spanMock.Object);

        var producerMock = new Mock<IKafkaProducer<TransationCreated>>();
        producerMock
            .Setup(p => p.PublishAsync(It.IsAny<TransationCreated>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTransactionHandler(loggerMock.Object, tracerMock.Object, producerMock.Object);

        var command = new CreateTransactionCommand
        {
            Amount = 100.0m,
            Currency = "USD",
            Description = "Test transaction",
            SenderAccountId = "sender-123",
            ReceiverAccountId = "receiver-456"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Result);
        producerMock.Verify(p => p.PublishAsync(It.Is<TransationCreated>(e =>
            e.Amount == command.Amount &&
            e.Currency == command.Currency &&
            e.Description == command.Description &&
            e.SenderAccountId == command.SenderAccountId &&
            e.ReceiverAccountId == command.ReceiverAccountId
        ), It.IsAny<CancellationToken>()), Times.Once);

        spanMock.Verify(s => s.NewMessage(It.IsAny<string>()), Times.Once);
        spanMock.Verify(s => s.Success(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetTransactionIdAndDate()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateTransactionHandler>>();
        var tracerMock = new Mock<ITracer>();
        var spanMock = new Mock<ISpan>();
        tracerMock.Setup(t => t.Span<CreateTransactionHandler>()).Returns(spanMock.Object);

        var producerMock = new Mock<IKafkaProducer<TransationCreated>>();
        producerMock
            .Setup(p => p.PublishAsync(It.IsAny<TransationCreated>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTransactionHandler(loggerMock.Object, tracerMock.Object, producerMock.Object);

        var command = new CreateTransactionCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, command.TransactionId);
        Assert.True((DateTime.UtcNow - command.TransactionDate).TotalSeconds < 5);
    }
}
