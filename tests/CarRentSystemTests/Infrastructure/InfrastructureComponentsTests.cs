using System.Text.Json;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Infrastructure.Persistence;
using CarRentSystem.Infrastructure.Serialization;

namespace CarRentSystem.Tests.Infrastructure;

public class FileStorageTests
{
    [Fact]
    public async Task SaveAsync_Without_Open_Should_Throw()
    {
        var storage = new FileStorage();

        await Assert.ThrowsAsync<InvalidOperationException>(() => storage.SaveAsync(new List<Car>()));
    }

    [Fact]
    public async Task LoadAsync_Without_Open_Should_Throw()
    {
        var storage = new FileStorage();

        await Assert.ThrowsAsync<InvalidOperationException>(() => storage.LoadAsync());
    }

    [Fact]
    public async Task LoadAsync_Should_Return_Empty_List_When_File_Does_Not_Exist()
    {
        var filePath = CreateTempFilePath();
        using var storage = new FileStorage();
        storage.Open(filePath);

        var cars = await storage.LoadAsync();

        Assert.Empty(cars);
    }

    [Fact]
    public async Task SaveAsync_And_LoadAsync_Should_RoundTrip_Cars()
    {
        var filePath = CreateTempFilePath();
        using var storage = new FileStorage();
        storage.Open(filePath);

        try
        {
            var cars = new List<Car>
            {
                new("BMW X5"),
                new("Custom Van", 95)
            };

            await storage.SaveAsync(cars);
            var loaded = await storage.LoadAsync();

            Assert.Equal(2, loaded.Count);
            Assert.Contains(loaded, car => car.Model == "BMW X5" && car.PricePerDay == 120);
            Assert.Contains(loaded, car => car.Model == "Custom Van" && car.PricePerDay == 95);
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    [Fact]
    public async Task LoadAsync_Should_Return_Empty_List_For_Corrupted_Json()
    {
        var filePath = CreateTempFilePath();
        await File.WriteAllTextAsync(filePath, "{not valid json");

        using var storage = new FileStorage();
        storage.Open(filePath);

        try
        {
            var cars = await storage.LoadAsync();

            Assert.Empty(cars);
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    [Fact]
    public void Dispose_Should_Be_Safe_To_Call_Multiple_Times()
    {
        var storage = new FileStorage();

        storage.Dispose();
        storage.Dispose();
    }

    private static string CreateTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), $"cars_{Guid.NewGuid():N}.json");
    }

    private static void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}

public class JsonServiceTests
{
    [Fact]
    public void Serialize_Should_Return_Json_For_Object()
    {
        var service = new JsonService();

        var json = service.Serialize(new SampleDto(7, "Roadster"));

        Assert.Contains("\"Id\":7", json);
        Assert.Contains("\"Name\":\"Roadster\"", json);
    }

    [Fact]
    public void Serialize_Should_Return_Null_Literal_For_Null_Object()
    {
        var service = new JsonService();

        var json = service.Serialize<object?>(null);

        Assert.Equal("null", json);
    }

    [Fact]
    public void Deserialize_Should_Create_Object_From_Json()
    {
        var service = new JsonService();

        var result = service.Deserialize<SampleDto>("{\"Id\":5,\"Name\":\"Coupe\"}");

        Assert.Equal(5, result.Id);
        Assert.Equal("Coupe", result.Name);
    }

    [Fact]
    public void Deserialize_Should_Throw_For_Invalid_Json()
    {
        var service = new JsonService();

        Assert.Throws<JsonException>(() => service.Deserialize<SampleDto>("{bad json"));
    }

    private sealed record SampleDto(int Id, string Name);
}

public class JsonDataStoreResultTests
{
    [Fact]
    public void LoadResult_Should_Return_Success_With_Empty_List_When_File_Is_Missing()
    {
        var result = JsonDataStore<string>.LoadResult(CreateTempFilePath());

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public void LoadResult_Should_Return_Success_For_Valid_Json()
    {
        var filePath = CreateTempFilePath();

        try
        {
            File.WriteAllText(filePath, "[\"a\",\"b\"]");

            var result = JsonDataStore<string>.LoadResult(filePath);

            Assert.True(result.Success);
            Assert.Equal(new[] { "a", "b" }, result.Value);
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    [Fact]
    public void LoadResult_Should_Return_Failure_For_Invalid_Json()
    {
        var filePath = CreateTempFilePath();

        try
        {
            File.WriteAllText(filePath, "invalid json content");

            var result = JsonDataStore<string>.LoadResult(filePath);

            Assert.False(result.Success);
            Assert.Contains("Corrupted JSON", result.ErrorMessage);
            Assert.Null(result.Value);
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    [Fact]
    public void Load_Should_Return_Value_From_LoadResult()
    {
        var filePath = CreateTempFilePath();

        try
        {
            File.WriteAllText(filePath, "[1,2,3]");

            var result = JsonDataStore<int>.Load(filePath);

            Assert.Equal(new[] { 1, 2, 3 }, result);
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    [Fact]
    public void Save_Should_Write_File_And_Return_Success()
    {
        var filePath = CreateTempFilePath();

        try
        {
            var result = JsonDataStore<int>.Save(filePath, new[] { 1, 2, 3 });

            Assert.True(result.Success);
            Assert.True(File.Exists(filePath));
            Assert.Contains("1", File.ReadAllText(filePath));
        }
        finally
        {
            DeleteIfExists(filePath);
        }
    }

    private static string CreateTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), $"store_{Guid.NewGuid():N}.json");
    }

    private static void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
