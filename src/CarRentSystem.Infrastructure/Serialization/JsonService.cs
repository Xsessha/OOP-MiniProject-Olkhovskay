using System.Text.Json;

namespace CarRentSystem.Infrastructure.Serialization;

public class JsonService
{
    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }
}