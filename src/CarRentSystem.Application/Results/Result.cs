namespace CarRentSystem.Application.Results;

public class Result
{
    public bool Success { get; init; }
    public bool IsSuccess => Success;
    public bool IsFailure => !Success;
    public string? ErrorMessage { get; init; }

    protected Result() { }

    public static Result Ok() => new Result { Success = true };
    public static Result Fail(string errorMessage) => new Result { Success = false, ErrorMessage = errorMessage };
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new Result<T> { Success = true, Value = value };
    public static new Result<T> Fail(string errorMessage) => new Result<T> { Success = false, ErrorMessage = errorMessage };
}
