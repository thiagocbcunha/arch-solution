namespace Verx.Authentication.Service.Application.ValidateUserToken;

/// <summary>
/// Result of token validation.
/// </summary>
public record ValidateTokenUserResult(bool IsValid, IDictionary<string, string>? Claims, string? Error);