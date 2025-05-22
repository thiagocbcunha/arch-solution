using FluentValidation.Results;

namespace Verx.Autentication.Service.Api.Common;

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
