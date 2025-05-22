using MediatR;

namespace Verx.Authentication.Service.Application.LoginUser;

/// <summary>
/// Command to login a user.
/// </summary>
public record LoginUserCommand : IRequest<LoginUserResult>
{
    /// <summary>
    /// The email of the user to be logged in.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The password for the user to be logged in.
    /// </summary>
    public required string Password { get; set; }
}
