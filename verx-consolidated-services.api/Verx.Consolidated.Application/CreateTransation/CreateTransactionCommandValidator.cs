using FluentValidation;

namespace Verx.Consolidated.Application.CreateTransation;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required.")
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3, 3)
            .WithMessage("Currency must be a 3-letter ISO currency code.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(255)
            .WithMessage("Description cannot exceed 255 characters.");
        RuleFor(x => x.SenderAccountId)
            .NotEmpty()
            .WithMessage("Sender account ID is required.");
        RuleFor(x => x.ReceiverAccountId)
            .NotEmpty()
            .WithMessage("Receiver account ID is required.");
    }
}