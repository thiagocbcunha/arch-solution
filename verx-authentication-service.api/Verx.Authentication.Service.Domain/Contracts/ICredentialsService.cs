using Microsoft.AspNetCore.Identity;

namespace Verx.Authentication.Service.Domain.Contracts;

/// <summary>
/// Interface for user registration service.
/// </summary>
public interface ICredentialsService
{
    /// <summary>
    /// Registers a new user with the specified email and password.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<IdentityResult> RegisterUserAsync(string email, string password);

    /// <summary>
    /// Validates the user credentials and returns a JWT token if valid.
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <returns>JWT token string if valid; otherwise, null.</returns>
    Task<string?> ValidateAndGenerateTokenAsync(string email, string password);

    /// <summary>
    /// Validates a JWT token and returns its claims if valid.
    /// </summary>
    /// <param name="token">JWT token string</param>
    /// <returns>Dictionary of claim types and values if valid; otherwise, throws exception.</returns>
    Task<IDictionary<string, string>> ValidateTokenAsync(string token);
}
