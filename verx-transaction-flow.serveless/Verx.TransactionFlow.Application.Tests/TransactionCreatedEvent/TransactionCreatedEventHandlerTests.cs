using Moq;
using System.Net;
using Moq.Protected;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verx.TransactionFlow.Domain.Options;
using Verx.TransactionFlow.Application.ProcessEvent;

namespace Verx.TransactionFlow.Application.Tests.TransactionCreatedEvent;

public class TransactionCreatedEventHandlerTests
{
    private TransactionCreatedEventHandler CreateHandler(HttpResponseMessage response)
    {
        // Mock HttpMessageHandler to control HttpClient behavior
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var loggerMock = new Mock<ILogger<TransactionCreatedEventHandler>>();
        var tracerMock = new Mock<ITracer>();
        var spanMock = new Mock<ISpan>();
        tracerMock.Setup(t => t.Span<TransactionCreatedEventHandler>()).Returns(spanMock.Object);

        var optionsMock = new Mock<IOptions<ConsolidatedSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new ConsolidatedSettings { UrlBase = "http://localhost" });

        return new TransactionCreatedEventHandler(
            loggerMock.Object,
            optionsMock.Object,
            tracerMock.Object,
            httpClientFactoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenHttpResponseIsSuccess()
    {
        // Arrange
        var handler = CreateHandler(new HttpResponseMessage(HttpStatusCode.OK));
        var command = new TransactionCreatedEventCommand
        {
            TransactionId = Guid.NewGuid(),
            Amount = 10,
            Currency = "USD",
            Description = "desc",
            SenderAccountId = "s",
            ReceiverAccountId = "r",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Result);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenHttpResponseIsFailure()
    {
        // Arrange
        var handler = CreateHandler(new HttpResponseMessage(HttpStatusCode.BadRequest));
        var command = new TransactionCreatedEventCommand
        {
            TransactionId = Guid.NewGuid(),
            Amount = 10,
            Currency = "USD",
            Description = "desc",
            SenderAccountId = "s",
            ReceiverAccountId = "r",
            TransactionDate = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
