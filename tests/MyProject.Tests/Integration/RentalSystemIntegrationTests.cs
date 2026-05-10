using Xunit;
using System;
using System.IO;
using System.Linq;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Persistence;
using MyProject.Infrastructure.Repositories;
using MyProject.Application.Services;

namespace MyProject.Tests.Integration;

public class RentalSystemIntegrationTests
{


    [Fact]
    public void Should_Create_Cars_Data()
    {
        var car = new Car("BMW");

        Assert.NotNull(car);
        Assert.Equal("BMW", car.Model);
    }



    [Fact]
    public void Should_Save_Data_To_File()
    {
        var path = Path.GetTempFileName();

        var cars = new[] { new Car("BMW") };

        JsonDataStore<Car>.Save(path, cars);

        Assert.True(File.Exists(path));
    }



    [Fact]
    public void Should_Load_Data_From_File()
    {
        var path = Path.GetTempFileName();

        var cars = new[] { new Car("BMW"), new Car("Audi") };

        JsonDataStore<Car>.Save(path, cars);

        var loaded = JsonDataStore<Car>.Load(path);

        Assert.Equal(2, loaded.Count);
    }



    [Fact]
    public void Should_Rent_After_Reload()
    {
        var path = Path.GetTempFileName();

        var car = new Car("BMW");
        JsonDataStore<Car>.Save(path, new[] { car });

        var loaded = JsonDataStore<Car>.Load(path);

        var repo = new InMemoryCarRepository(loaded);
        var service = new RentalService(repo, new InMemoryRentalRepository());

        service.RentCar("User", "economy", loaded[0].Id, 3);

        Assert.False(loaded[0].IsAvailable);
    }



    [Fact]
    public void Should_Return_After_Restore()
    {
        var path = Path.GetTempFileName();

        var car = new Car("BMW");
        JsonDataStore<Car>.Save(path, new[] { car });

        var loaded = JsonDataStore<Car>.Load(path);

        var repo = new InMemoryCarRepository(loaded);
        var service = new RentalService(repo, new InMemoryRentalRepository());

        service.RentCar("User", "economy", car.Id, 2);
        service.ReturnCar(car.Id);

        Assert.True(car.IsAvailable);
    }

  

    [Fact]
    public void Should_Handle_Multiple_Sequential_Operations()
    {
        var repo = new InMemoryCarRepository();
        var service = new RentalService(repo, new InMemoryRentalRepository());

        var c1 = new Car("BMW");
        var c2 = new Car("Audi");

        repo.Add(c1);
        repo.Add(c2);

        service.RentCar("User", "economy", c1.Id, 1);
        service.RentCar("User", "economy", c2.Id, 2);
        service.ReturnCar(c1.Id);

        Assert.True(c1.IsAvailable);
        Assert.False(c2.IsAvailable);
    }



    [Fact]
    public void Should_Handle_Empty_File()
    {
        var path = Path.GetTempFileName();

        var result = JsonDataStore<Car>.Load(path);

        Assert.NotNull(result);
    }

 

    [Fact]
    public void Should_Handle_Missing_File()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        var result = JsonDataStore<Car>.Load(path);

        Assert.NotNull(result);
    }

  

    [Fact]
    public void Should_Handle_Corrupted_File()
    {
        var path = Path.GetTempFileName();

        File.WriteAllText(path, "NOT_JSON");

        var result = JsonDataStore<Car>.Load(path);

        Assert.NotNull(result);
    }


    [Fact]
    public void Should_Persist_Multiple_Cycles()
    {
        var path = Path.GetTempFileName();

        var car = new Car("BMW");

        for (int i = 0; i < 3; i++)
        {
            JsonDataStore<Car>.Save(path, new[] { car });
            var loaded = JsonDataStore<Car>.Load(path);

            Assert.Single(loaded);
        }
    }


    [Fact]
    public void Should_Rent_After_SaveLoad_Cycle()
    {
        var path = Path.GetTempFileName();

        var car = new Car("BMW");
        JsonDataStore<Car>.Save(path, new[] { car });

        var loaded = JsonDataStore<Car>.Load(path);

        loaded[0].Rent();

        Assert.False(loaded[0].IsAvailable);
    }


    [Fact]
    public void Full_System_Flow_Should_Work()
    {
        var path = Path.GetTempFileName();

        var cars = new[]
        {
            new Car("BMW"),
            new Car("Audi")
        };

        JsonDataStore<Car>.Save(path, cars);

        var loaded = JsonDataStore<Car>.Load(path);

        var repo = new InMemoryCarRepository(loaded);
        var service = new RentalService(repo, new InMemoryRentalRepository());

        service.RentCar("User", "economy", loaded[0].Id, 3);
        service.RentCar("User", "premium", loaded[1].Id, 2);

        service.ReturnCar(loaded[0].Id);

        Assert.True(loaded[0].IsAvailable);
        Assert.False(loaded[1].IsAvailable);
    }
}