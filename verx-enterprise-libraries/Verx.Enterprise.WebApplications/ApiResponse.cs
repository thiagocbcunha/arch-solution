using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Verx.Enterprise.WebApplications;

[ExcludeFromCodeCoverage]
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

[ExcludeFromCodeCoverage]
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<ValidationFailure> Errors { get; set; } = [];
}

[ExcludeFromCodeCoverage]
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
    }
}