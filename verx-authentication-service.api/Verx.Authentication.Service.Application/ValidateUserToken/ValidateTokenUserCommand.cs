using MediatR;

namespace Verx.Authentication.Service.Application.ValidateUserToken;

/// <summary>
/// Command to validate a JWT token.
/// </summary>
public record ValidateUserTokenCommand(string Token) : IRequest<ValidateTokenUserResult>;
