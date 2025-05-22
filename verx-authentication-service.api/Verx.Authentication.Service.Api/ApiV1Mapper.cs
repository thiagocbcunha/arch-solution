using MediatR;
using Verx.Autentication.Service.Api.Common;
using Verx.Authentication.Service.Application.LoginUser;
using Verx.Authentication.Service.Application.CreateUser;
using Verx.Authentication.Service.Application.ValidateUserToken;

namespace Verx.Autentication.Service.Api;

public static class ApiV1Mapper
{
    public static void MapV1Endpoints(this WebApplication app)
    {
        var apiV1 = app.MapGroup("/api/v1");

        apiV1.WithTags("Authentication Service API V1");

        apiV1.MapPost("/users", async (CreateUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return CreateOk(result);
        })
        .WithOpenApi()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<CreateUserResult>>(StatusCodes.Status201Created);

        apiV1.MapPost("/login", async (LoginUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return CreateOk(result);
        })
        .WithOpenApi()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<LoginUserResult>>(StatusCodes.Status200OK);

        apiV1.MapPost("/validate-token", async (ValidateUserTokenCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return CreateOk(result);
        })
        .WithOpenApi()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces<ApiResponse<ValidateTokenUserResult>>(StatusCodes.Status200OK);
    }

    private static IResult CreateOk<T>(this  T result)
    {
        var response = new ApiResponse<T>
        {
            Message = "Success",
            Success = true,
            Data = result
        };

        return Results.Ok(response);
    }
}
