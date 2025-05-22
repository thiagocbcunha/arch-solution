using System.Text.Json;
using FluentValidation.Results;

namespace Verx.Consolidated.Onboarding.Command.Api.Middware;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}


public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<ValidationFailure> Errors { get; set; } = [];
}


public static class ApiResponseExtension
{
    public static async Task Ok<T>(this HttpContext context, T data)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var response = new ApiResponse<T>
        {
            Data = data,
            Success = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        await context.Response.CompleteAsync();
    }
}