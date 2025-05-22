using MediatR;
using Microsoft.Extensions.Logging;
using Verx.Authentication.Service.Domain.Contracts;

namespace Verx.Authentication.Service.Application.LoginUser;

/// <summary>
/// Handles the login process for a user by validating credentials and generating a JWT token.
/// </summary>
/// <remarks>
/// This handler receives a <see cref="LoginUserCommand"/> containing the user's email and password,
/// validates the credentials using the <see cref="ICredentialsService"/>, and returns a <see cref="LoginUserResult"/>
/// indicating whether authentication was successful and, if so, the generated JWT token.
/// </remarks>
public class LoginUserHandler(ILogger<LoginUserHandler> logger, ICredentialsService credentialsService) : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    /// <summary>
    /// Handles the login user command by validating the provided credentials and generating a JWT token if valid.
    /// </summary>
    /// <param name="request">The login user command containing the user's email and password.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns>
    /// A <see cref="LoginUserResult"/> containing a success flag and the generated JWT token if authentication is successful; otherwise, a failure result.
    /// </returns>
    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Logging in user with email: {Email}", request.Email);

        var token = await credentialsService.ValidateAndGenerateTokenAsync(request.Email, request.Password);
        return new LoginUserResult(true, token);
    }
}
