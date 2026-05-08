using System.Text.Json;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Persistence;

public class FileStorage : IDisposable
{
    private string? _path;

    public void Open(string path)
    {
        _path = path;
    }

    // 🔥 SAVE (JSON)
    public async Task SaveAsync(List<Car> cars)
    {
        if (_path is null)
            throw new InvalidOperationException("File path is not set.");

        var json = JsonSerializer.Serialize(cars, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_path, json);
    }

    // 🔥 LOAD (JSON)
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
        catch (JsonException)
        {
            // пошкоджений файл
            return new List<Car>();
        }
    }

    public void Dispose()
    {
        // зараз нічого не тримаєш відкритим довго
    }
}