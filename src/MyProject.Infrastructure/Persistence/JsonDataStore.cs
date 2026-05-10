using System.Text.Json;
using System.Threading;
using MyProject.Application.Events;

namespace MyProject.Infrastructure.Persistence;

public class Result
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public static Result Ok() => new Result { Success = true };
    public static Result Fail(string errorMessage) => new Result { Success = false, ErrorMessage = errorMessage };
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Ok(T value) => new Result<T> { Success = true, Value = value };
    public static new Result<T> Fail(string errorMessage) => new Result<T> { Success = false, ErrorMessage = errorMessage };
}

public static class JsonDataStore<T>
{
    private const int MaxSaveAttempts = 3;
    private const int RetryDelayMilliseconds = 100;

    public static Result Save(string path, IEnumerable<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        for (var attempt = 1; attempt <= MaxSaveAttempts; attempt++)
        {
            try
            {
                File.WriteAllText(path, json);
                return Result.Ok();
            }
            catch (IOException ex)
            {
                var message = $"Failed to save file '{path}' (attempt {attempt}/{MaxSaveAttempts}): {ex.Message}";
                ApplicationEventBus.Notify(message);

                if (attempt == MaxSaveAttempts)
                {
                    return Result.Fail(message);
                }

                Thread.Sleep(RetryDelayMilliseconds);
            }
        }

        return Result.Fail($"Unable to save file '{path}' after {MaxSaveAttempts} attempts.");
    }

    public static List<T> Load(string path)
    {
        return LoadResult(path).Value ?? new List<T>();
    }

    public static Result<List<T>> LoadResult(string path)
    {
        if (!File.Exists(path))
        {
            return Result<List<T>>.Ok(new List<T>());
        }

        try
        {
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<T>>(json)
                       ?? new List<T>();
            return Result<List<T>>.Ok(data);
        }
        catch (JsonException ex)
        {
            var message = $"Corrupted JSON in '{path}': {ex.Message}";
            ApplicationEventBus.Notify(message);
            return Result<List<T>>.Fail(message);
        }
        catch (IOException ex)
        {
            var message = $"I/O error while reading '{path}': {ex.Message}";
            ApplicationEventBus.Notify(message);
            return Result<List<T>>.Fail(message);
        }
    }
}