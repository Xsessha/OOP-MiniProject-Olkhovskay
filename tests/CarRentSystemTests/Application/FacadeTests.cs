using Xunit;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Infrastructure.Repositories;
using CarRentSystem.Application.Services;
using CarRentSystem.Application.Facades;

namespace CarRentSystem.Tests.Application;

public class FacadeTests
{
    private RentalFacade CreateFacade()
    {
        var cars = new List<Car>
        {
            new Car("BMW"),
            new Car("Audi")
        };

        var carRepo = new CarRepository(cars);
        var rentalRepo = new InMemoryRentalRepository();
        var service = new RentalService(carRepo, rentalRepo);

        return new RentalFacade(service, carRepo, rentalRepo);
    }

    [Fact]
    public void Facade_Should_Return_All_Cars()
    {
        var facade = CreateFacade();

        var cars = facade.GetCars();

        Assert.Equal(2, cars.Count);
    }

    [Fact]
    public void Facade_Should_Filter_Available_Cars()
    {
        var facade = CreateFacade();

        var cars = facade.GetAvailableCars();

        Assert.All(cars, c => Assert.True(c.IsAvailable));
    }

    [Fact]
    public void Facade_Should_Rent_Car()
    {
        var facade = CreateFacade();

        var car = facade.GetCars().First();

        facade.Rent("John", "economy", car.Id, 3);

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void Facade_Should_Return_Car()
    {
        var facade = CreateFacade();

        var car = facade.GetCars().First();

        facade.Rent("John", "economy", car.Id, 3);

        facade.Return(car.Id);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Facade_Should_Calculate_Revenue_After_Rental()
    {
        var facade = CreateFacade();
        var car = facade.GetCars().First();

        // Before rental, revenue should be 0
        Assert.Equal(0, facade.GetRevenue());

        // Rent car for 3 days as economy customer (5% discount)
        facade.Rent("John", "economy", car.Id, 3);

        // Return car
        facade.Return(car.Id);

        // After return, revenue should be car price * days * (1 - economy discount)
        // Economy customer gets 5% discount, so revenue = 60 * 3 * 0.95 = 171
        var expectedRevenue = car.PricePerDay * 3 * 0.95m;
        Assert.Equal(expectedRevenue, facade.GetRevenue());
    }
}