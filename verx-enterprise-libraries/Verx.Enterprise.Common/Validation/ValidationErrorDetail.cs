using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.Common.Validation;

[ExcludeFromCodeCoverage]
public class ValidationErrorDetail
{
    public string Error { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;

    public static explicit operator ValidationErrorDetail(ValidationFailure validationFailure)
    {
        return new ValidationErrorDetail
        {
            Error = validationFailure.ErrorCode,
            Detail = validationFailure.ErrorMessage
        };
    }
}
