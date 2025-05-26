using Moq;
using FluentAssertions;
using AutoFixture.Xunit2;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Verx.Consolidated.Domain.Dtos;
using Verx.Consolidated.Domain.Entities;
using Verx.Consolidated.Domain.Contracts;
using Verx.Enterprise.MessageBroker.RabbitMQ;
using Verx.Consolidated.Application.CreateTransation;

namespace Verx.Consolidated.Application.Tests.CreateTransaction;

public class CreateTransactionHandlerTests
{
    [Theory, AutoData]
    public async Task Handle_ShouldCreateTransactionAndPublishConsolidatedDto(CreateTransactionCommand command, decimal totalAmount)
    {
        // Arrange
        var spanMock = new Mock<ISpan>();
        var tracerMock = new Mock<ITracer>();
        var loggerMock = new Mock<ILogger<CreateTransactionHandler>>();
        tracerMock.Setup(t => t.Span<CreateTransactionHandler>()).Returns(spanMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(ITracer))).Returns(tracerMock.Object);

        var transactionRepositoryMock = new Mock<ITransactionRepository>();
        transactionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TransactionEntity>())).Returns(Task.CompletedTask);
        transactionRepositoryMock.Setup(r => r.GetTotalAmountByAccountId(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(totalAmount);

        var rabbitProducerMock = new Mock<IRabbitProducer<ConsolidatedDto>>();
        rabbitProducerMock.Setup(r => r.PublishAsync(It.IsAny<ConsolidatedDto>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var handler = new CreateTransactionHandler(loggerMock.Object, serviceProviderMock.Object, transactionRepositoryMock.Object, rabbitProducerMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeTrue();

        transactionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TransactionEntity>()), Times.Once);
        transactionRepositoryMock.Verify(r => r.GetTotalAmountByAccountId(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        rabbitProducerMock.Verify(r => r.PublishAsync(It.IsAny<ConsolidatedDto>(), It.IsAny<CancellationToken>()), Times.Once);
        spanMock.Verify(s => s.NewMessage(It.IsAny<string>()), Times.Once);
        spanMock.Verify(s => s.Success(), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_ShouldLogInformation(
        CreateTransactionCommand command)
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CreateTransactionHandler>>();
        var tracerMock = new Mock<ITracer>();
        var spanMock = new Mock<ISpan>();
        tracerMock.Setup(t => t.Span<CreateTransactionHandler>()).Returns(spanMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(ITracer))).Returns(tracerMock.Object);

        var transactionRepositoryMock = new Mock<ITransactionRepository>();
        transactionRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TransactionEntity>()))
            .Returns(Task.CompletedTask);
        transactionRepositoryMock
            .Setup(r => r.GetTotalAmountByAccountId(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(100m);

        var rabbitProducerMock = new Mock<IRabbitProducer<ConsolidatedDto>>();
        rabbitProducerMock
            .Setup(r => r.PublishAsync(It.IsAny<ConsolidatedDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTransactionHandler(
            loggerMock.Object,
            serviceProviderMock.Object,
            transactionRepositoryMock.Object,
            rabbitProducerMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Creating transaction")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}