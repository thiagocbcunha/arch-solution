using FluentValidation.TestHelper;
using Verx.TransactionFlow.Application.ProcessEvent;

namespace Verx.TransactionFlow.Application.Tests.TransactionCreatedEvent;

public class TransactionCreatedEventCommandValidatorTests
{
    private readonly TransactionCreatedEventCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_Validation_For_Valid_Command()
    {
        var command = new TransactionCreatedEventCommand
        {
            Amount = 100.0m,
            Currency = "USD",
            Description = "Valid description",
            SenderAccountId = "sender-123",
            ReceiverAccountId = "receiver-456"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Amount_Is_Zero_Or_Negative()
    {
        var command = new TransactionCreatedEventCommand
        {
            Amount = 0,
            Currency = "USD",
            Description = "desc",
            SenderAccountId = "s",
            ReceiverAccountId = "r"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);

        command.Amount = -10;
        result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_Fail_When_Currency_Is_Invalid()
    {
        var command = new TransactionCreatedEventCommand
        {
            Amount = 10,
            Currency = "",
            Description = "desc",
            SenderAccountId = "s",
            ReceiverAccountId = "r"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Currency);

        command.Currency = "US";
        result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Currency);

        command.Currency = "USDT";
        result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Should_Fail_When_Description_Is_Empty_Or_TooLong()
    {
        var command = new TransactionCreatedEventCommand
        {
            Amount = 10,
            Currency = "USD",
            Description = "",
            SenderAccountId = "s",
            ReceiverAccountId = "r"
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);

        command.Description = new string('a', 256);
        result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Fail_When_SenderAccountId_Or_ReceiverAccountId_Is_Empty()
    {
        var command = new TransactionCreatedEventCommand
        {
            Amount = 10,
            Currency = "USD",
            Description = "desc",
            SenderAccountId = "",
            ReceiverAccountId = ""
        };

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SenderAccountId);
        result.ShouldHaveValidationErrorFor(x => x.ReceiverAccountId);
    }
}
