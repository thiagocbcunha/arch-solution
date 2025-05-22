using MediatR;
using Microsoft.Extensions.Logging;
using Verx.Authentication.Service.Domain.Contracts;

namespace Verx.Authentication.Service.Application.CreateUser;

/// <summary>
/// Handler for the CreateUserCommand.
/// </summary>
/// <param name="userRegistrationService"></param>
public class CreateUserHandler(ILogger<CreateUserHandler> logger, ICredentialsService userRegistrationService) : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    /// <summary>
    /// Handles the CreateUserCommand.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user with email: {Email}", request.Email);

        await userRegistrationService.RegisterUserAsync(request.Email, request.Password);
        return new CreateUserResult(true);
    }
}