using MediatR;
using Microsoft.Extensions.Logging;
using Verx.Authentication.Service.Domain.Contracts;

namespace Verx.Authentication.Service.Application.ValidateUserToken;

/// <summary>
/// Handler for ValidateTokenCommand.
/// </summary>
public class ValidateTokenUserHandler(ILogger<ValidateTokenUserHandler> logger, ICredentialsService credentialsService) : IRequestHandler<ValidateUserTokenCommand, ValidateTokenUserResult>
{
    /// <summary>
    /// Handles the ValidateTokenCommand and returns the result.
    /// </summary>
    public async Task<ValidateTokenUserResult> Handle(ValidateUserTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Validating token: {Token}", request.Token);

        var claims = await credentialsService.ValidateTokenAsync(request.Token);
        return new ValidateTokenUserResult(true, claims, null);
    }
}