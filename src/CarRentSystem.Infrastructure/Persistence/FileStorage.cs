using System.Text.Json;
using CarRentSystem.Application.Events;
using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Infrastructure.Persistence;

public class FileStorage : IDisposable
{
    private string? _path;

    public void Open(string path)
    {
        _path = path;
    }

    public async Task SaveAsync(List<Car> cars)
    {
        if (_path is null)
            throw new InvalidOperationException("File path is not set.");

        var json = JsonSerializer.Serialize(cars, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        try
        {
            await File.WriteAllTextAsync(_path, json);
        }
        catch (IOException ex)
        {
            ApplicationEventBus.Notify($"I/O error while saving '{_path}': {ex.Message}");
            throw;
        }
    }

    public async Task<List<Car>> LoadAsync()
    {
        if (_path is null)
            throw new InvalidOperationException("File path is not set.");

        if (!File.Exists(_path))
            return new List<Car>();

        try
        {
            var json = await File.ReadAllTextAsync(_path);

            return JsonSerializer.Deserialize<List<Car>>(json)
                   ?? new List<Car>();
        }
        catch (JsonException ex)
        {
            ApplicationEventBus.Notify($"Corrupted JSON in '{_path}': {ex.Message}");
            return new List<Car>();
        }
        catch (IOException ex)
        {
            ApplicationEventBus.Notify($"I/O error while loading '{_path}': {ex.Message}");
            return new List<Car>();
        }
    }

    public void Dispose()
    {
    }
}