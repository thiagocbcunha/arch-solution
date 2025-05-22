using System.Text.Json.Serialization;
using Verx.Authentication.Service.Application.LoginUser;
using Verx.Authentication.Service.Application.CreateUser;
using Verx.Authentication.Service.Application.ValidateUserToken;

namespace Verx.Authentication.Service.Application;

[JsonSerializable(typeof(LoginUserCommand))]
[JsonSerializable(typeof(CreateUserCommand))]
[JsonSerializable(typeof(ValidateUserTokenCommand))]
public partial class ApplicationJsonSerializaerContext : JsonSerializerContext
{
}