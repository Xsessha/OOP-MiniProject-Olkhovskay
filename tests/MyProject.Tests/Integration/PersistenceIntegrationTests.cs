using Xunit;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Persistence;

namespace MyProject.Tests.Integration;

public class PersistenceIntegrationTests
{
    [Fact]
    public void Save_And_Load_Should_Preserve_Data()
    {
        var tempFile = Path.GetTempFileName();

        var cars = new List<Car>
        {
            new Car("BMW"),
            new Car("Audi")
        };

        JsonDataStore<Car>.Save(tempFile, cars);

        var loaded = JsonDataStore<Car>.Load(tempFile);

        Assert.Equal(2, loaded.Count);
    }

    [Fact]
    public void Loaded_Car_Should_Keep_Model()
    {
        var tempFile = Path.GetTempFileName();

        var cars = new List<Car>
        {
            new Car("Tesla")
        };

        JsonDataStore<Car>.Save(tempFile, cars);

        var loaded = JsonDataStore<Car>.Load(tempFile);

        Assert.Equal("Tesla", loaded[0].Model);
    }

    [Fact]
    public void Missing_File_Should_Return_Empty_List()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        var cars = JsonDataStore<Car>.Load(path);

        Assert.Empty(cars);
    }

    [Fact]
    public void Multiple_Save_Operations_Should_Work()
    {
        var tempFile = Path.GetTempFileName();

        var cars1 = new List<Car>
        {
            new Car("BMW")
        };

        JsonDataStore<Car>.Save(tempFile, cars1);

        var cars2 = new List<Car>
        {
            new Car("Audi"),
            new Car("Tesla")
        };

        JsonDataStore<Car>.Save(tempFile, cars2);

        var loaded = JsonDataStore<Car>.Load(tempFile);

        Assert.Equal(2, loaded.Count);
    }
}