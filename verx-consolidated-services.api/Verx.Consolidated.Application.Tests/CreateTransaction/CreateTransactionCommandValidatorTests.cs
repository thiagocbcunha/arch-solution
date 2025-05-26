using FluentAssertions;
using Verx.Consolidated.Application.CreateTransation;

namespace Verx.Consolidated.Application.Tests.CreateTransaction;

public class CreateTransactionCommandValidatorTests
{
    private readonly CreateTransactionCommandValidator _validator = new();

    private CreateTransactionCommand CreateValidCommand() => new CreateTransactionCommand
    {
        TransactionId = Guid.NewGuid(),
        Amount = 100,
        Currency = "USD",
        Description = "Test transaction",
        SenderAccountId = "sender-123",
        ReceiverAccountId = "receiver-456",
        TransactionDate = DateTime.UtcNow
    };

    [Fact]
    public void Should_Pass_For_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_Fail_When_Amount_Is_Invalid(decimal amount)
    {
        var command = CreateValidCommand() with { Amount = amount };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("USDA")]
    public void Should_Fail_When_Currency_Is_Invalid(string currency)
    {
        var command = CreateValidCommand() with { Currency = currency };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Fail_When_Description_Is_Empty(string description)
    {
        var command = CreateValidCommand() with { Description = description };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_MaxLength()
    {
        var command = CreateValidCommand() with { Description = new string('a', 256) };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Fail_When_SenderAccountId_Is_Empty(string senderId)
    {
        var command = CreateValidCommand() with { SenderAccountId = senderId };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SenderAccountId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Should_Fail_When_ReceiverAccountId_Is_Empty(string receiverId)
    {
        var command = CreateValidCommand() with { ReceiverAccountId = receiverId };
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ReceiverAccountId");
    }
}
