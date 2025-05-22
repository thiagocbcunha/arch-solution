using MediatR;

namespace Verx.Authentication.Service.Application.CreateUser;

/// <summary>
/// Command to create a new user.
/// </summary>
public record CreateUserCommand : IRequest<CreateUserResult>
{
    /// <summary>
    /// The email of the user to be created.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The password for the user to be created.
    /// </summary>
    public required string Password { get; set; }
}