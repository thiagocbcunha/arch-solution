using Xunit;
using FluentAssertions;
using Verx.Consolidated.Domain.Entities;

namespace Verx.Consolidated.Domain.Tests.Entities;

public class TransactionEntityTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var amount = 100m;
        var date = DateTime.UtcNow;

        // Act
        var entity = new TransactionEntity { Amount = amount, TransactionDate = date };

        // Assert
        entity.Amount.Should().Be(amount);
        entity.TransactionDate.Should().Be(date);
    }
}
