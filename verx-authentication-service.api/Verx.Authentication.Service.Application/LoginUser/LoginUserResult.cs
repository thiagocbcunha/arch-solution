namespace Verx.Authentication.Service.Application.LoginUser;

/// <summary>
/// Result of a login attempt.
/// </summary>
public record LoginUserResult(bool Success, string? Token);
