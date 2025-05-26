using FluentAssertions;
using Moq;
using Verx.Consolidated.Application.GetAllConsolidated;
using Verx.Consolidated.Domain.Contracts;
using Verx.Consolidated.Domain.Dtos;
using Verx.Enterprise.Tracing;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Verx.Consolidated.Application.Tests.GetAllConsolidated;

public class GetAllConsolidatedHandlerTests
{
    private readonly Mock<ILogger<GetAllConsolidatedHandler>> _loggerMock = new();
    private readonly Mock<ITracer> _tracerMock = new();
    private readonly Mock<ISpan> _spanMock = new();
    private readonly Mock<IConsolidatedNSqlRepository> _repositoryMock = new();

    public GetAllConsolidatedHandlerTests()
    {
        _tracerMock.Setup(t => t.Span<GetAllConsolidatedHandler>()).Returns(_spanMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnConsolidatedResult_WithExpectedData()
    {
        // Arrange
        var date = new DateOnly(2024, 5, 25);
        var command = new GetAllConsolidatedCommand { Date = date };
        var expectedList = new List<ConsolidatedDto>
        {
            new() { Date = date, Total = 100, UpdateDate = DateTime.UtcNow }
        };

        _repositoryMock
         .Setup(r => r.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>()))
         .Returns((Expression<Func<ConsolidatedDto, bool>> expr) => expectedList.Where(expr.Compile()));

        var handler = new GetAllConsolidatedHandler(
            _loggerMock.Object,
            _tracerMock.Object,
            _repositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Consolidateds.Should().BeEquivalentTo(expectedList);

        _repositoryMock.Verify(r => r.GetMany(It.IsAny<Expression<Func<ConsolidatedDto, bool>>>()), Times.Once);
        _spanMock.Verify(s => s.NewMessage(It.IsAny<string>()), Times.Once);
        _spanMock.Verify(s => s.Success(), Times.Once);
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Getting all consolidated transactions")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
