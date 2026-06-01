using Xunit;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Exceptions;
using CarRentSystem.Infrastructure.Repositories;
using CarRentSystem.Application.Services;

namespace CarRentSystem.Tests.Integration;

public class FullFlowIntegrationTests
{
    [Fact]
    public void Full_Rent_And_Return_Flow_Should_Work()
    {
        var repo = new InMemoryCarRepository();

        var rentalRepo = new InMemoryRentalRepository();

        var service = new RentalService(repo, rentalRepo);

        var car = new Car("BMW");

        repo.Add(car);

        service.RentCar("John", "economy", car.Id, 3);

        Assert.False(car.IsAvailable);

        service.ReturnCar(car.Id);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Multiple_Cars_Should_Work_Independently()
    {
        var repo = new InMemoryCarRepository();

        var rentalRepo = new InMemoryRentalRepository();

        var service = new RentalService(repo, rentalRepo);

        var c1 = new Car("BMW");
        var c2 = new Car("Audi");

        repo.Add(c1);
        repo.Add(c2);

        service.RentCar("John", "economy", c1.Id, 3);

        Assert.False(c1.IsAvailable);
        Assert.True(c2.IsAvailable);
    }

    [Fact]
    public void Renting_Invalid_Car_Should_Throw()
    {
        var repo = new InMemoryCarRepository();

        var rentalRepo = new InMemoryRentalRepository();

        var service = new RentalService(repo, rentalRepo);

        Assert.Throws<CarNotFoundException>(() =>
            service.RentCar("John", "economy", Guid.NewGuid(), 3));
    }

    [Fact]
    public void Returning_Invalid_Car_Should_Throw()
    {
        var repo = new InMemoryCarRepository();

        var rentalRepo = new InMemoryRentalRepository();

        var service = new RentalService(repo, rentalRepo);

        Assert.Throws<CarNotFoundException>(() =>
            service.ReturnCar(Guid.NewGuid()));
    }
}