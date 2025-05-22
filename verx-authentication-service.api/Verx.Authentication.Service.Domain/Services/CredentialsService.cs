using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Verx.Autentication.Service.Domain.Entities;
using Verx.Authentication.Service.Domain.Options;
using Verx.Authentication.Service.Domain.Contracts;
using Verx.Authentication.Service.Domain.Exceptions;

namespace Verx.Authentication.Service.Domain.Services;

/// <summary>
/// Service for user registration.
/// </summary>
/// <param name="userManager"></param>
public class CredentialsService(UserManager<ApplicationUser> userManager, IOptions<JwtConfiguration> options) : ICredentialsService
{
    // <inheritdoc/>
    public async Task<IdentityResult> RegisterUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var userRegister = await userManager.FindByEmailAsync(email);
        if (userRegister != null)
            throw new UserAlreadyExistsException("User already exists.");

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new ValidationUserException("Invalid email or password", result.Errors);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string?> ValidateAndGenerateTokenAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null || !await userManager.CheckPasswordAsync(user, password))
            throw new InvalidPasswordException("Invalid email or password.");

        var claims = new[]
        {
            new Claim("Email", user.Email!),
            new Claim("Name", user.UserName!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds,
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            expires: DateTime.UtcNow.AddMinutes(options.Value.ExpirationInMinutes));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc/>
    public Task<IDictionary<string, string>> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(options.Value.Key);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Value.Issuer,
            ValidAudience = options.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Task.FromResult<IDictionary<string, string>>(claims);
        }
        catch (Exception ex)
        {
            throw new InvalidUserTokenException("Invalid token.", ex);
        }
    }
}