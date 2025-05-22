using FluentValidation;
using System.Text.Json;

namespace Verx.TransactionFlow.Serveless.Common;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException exception)
        {
            await HandlerArgumentOutOfRangeExceptionAsync(context, exception);
        }
        catch (Exception ex) when (ex is InvalidOperationException exception)
        {
            await HandlerInvalidOperationExceptionAsync(context, exception);
        }
        catch (Exception ex) when (ex is ValidationException exception)
        {
            await HandleValidationExceptionAsync(context, exception);
        }
        catch (Exception ex) when (ex is KeyNotFoundException exception)
        {
            await HandlerNotFountExceptionAsync(context, exception);
        }
        catch (Exception)
        {
            await HandlerGenericExceptionAsync(context);
        }
    }

    /// <summary>
    /// Handles argument out of range exceptions by returning a structured response with error details.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private static Task HandlerArgumentOutOfRangeExceptionAsync(HttpContext context, ArgumentOutOfRangeException ex)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = ex.Message,
        };

        return WriteResponse(context, response, StatusCodes.Status406NotAcceptable);
    }

    /// <summary>
    /// Handles invalid operation exceptions by returning a structured response with error details.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private static Task HandlerInvalidOperationExceptionAsync(HttpContext context, InvalidOperationException ex)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = ex.Message,
        };

        return WriteResponse(context, response, StatusCodes.Status406NotAcceptable);
    }

    /// <summary>
    /// Handles general exceptions by returning a structured response with error details.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static Task HandlerGenericExceptionAsync(HttpContext context)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = "An error occurred while processing your request.",
        };

        return WriteResponse(context, response, StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles not found exceptions by returning a structured response with error details.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private static Task HandlerNotFountExceptionAsync(HttpContext context, KeyNotFoundException ex)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = ex.Message
        };

        return WriteResponse(context, response, StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Handles validation exceptions by returning a structured response with error details.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = "Validation Failed",
            Errors = exception.Errors
        };

        return WriteResponse(context, response, StatusCodes.Status400BadRequest);
    }

    /// <summary>
    /// Writes the response to the HTTP context.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="response"></param>
    /// <param name="statusCodes"></param>
    /// <returns></returns>
    private static Task WriteResponse(HttpContext context, ApiResponse response, int statusCodes)
    {
        context.Response.StatusCode = statusCodes;
        context.Response.ContentType = "application/json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
