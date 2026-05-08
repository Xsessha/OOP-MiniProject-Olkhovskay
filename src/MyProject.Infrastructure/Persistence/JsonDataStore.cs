using System.Text.Json;

namespace MyProject.Infrastructure.Persistence;

public static class JsonDataStore<T>
{
    public static void Save(string path, List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(path, json);
    }

    public static List<T> Load(string path)
    {
        if (!File.Exists(path))
        {
            return new List<T>();
        }

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<T>>(json)
               ?? new List<T>();
    }
}