using Xunit;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using MyProject.Infrastructure.Repositories;
using MyProject.Application.Services;

namespace MyProject.Tests.Application;

public class RentalServiceTests
{
    private RentalService CreateService(out InMemoryCarRepository repo)
    {
        repo = new InMemoryCarRepository();
        return new RentalService(repo, new InMemoryRentalRepository());
    }

    [Fact]
    public void Should_Rent_Car()
    {
        var service = CreateService(out var repo);

        var car = new Car("BMW");
        repo.Add(car);

        service.RentCar("User", "economy", car.Id, 3);

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void Should_Return_Car()
    {
        var service = CreateService(out var repo);

        var car = new Car("BMW");
        repo.Add(car);

        service.RentCar("User", "economy", car.Id, 3);
        service.ReturnCar(car.Id);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Should_Handle_Multiple_Cars()
    {
        var service = CreateService(out var repo);

        var c1 = new Car("BMW");
        var c2 = new Car("Audi");

        repo.Add(c1);
        repo.Add(c2);

        service.RentCar("User", "economy", c1.Id, 3);

        Assert.False(c1.IsAvailable);
        Assert.True(c2.IsAvailable);
    }

    [Fact]
    public void Full_Flow_Should_Work()
    {
        var service = CreateService(out var repo);

        var car = new Car("BMW");
        repo.Add(car);

        service.RentCar("User", "economy", car.Id, 3);
        service.ReturnCar(car.Id);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Should_Not_Rent_Invalid_Car()
    {
        var service = CreateService(out var repo);

        Assert.Throws<CarNotFoundException>(() =>
            service.RentCar("User", "economy", Guid.NewGuid(), 3));
    }
}