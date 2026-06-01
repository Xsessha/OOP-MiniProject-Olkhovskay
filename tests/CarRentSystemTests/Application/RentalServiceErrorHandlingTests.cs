using Xunit;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Exceptions;
using CarRentSystem.Infrastructure.Repositories;
using CarRentSystem.Application.Services;

namespace CarRentSystem.Tests.Application;

public class RentalServiceErrorHandlingTests
{
    private RentalService CreateService(out InMemoryCarRepository repo)
    {
        repo = new InMemoryCarRepository();
        return new RentalService(repo, new InMemoryRentalRepository());
    }

    [Fact]
    public void Should_Throw_InvalidCustomerTypeException_For_Invalid_Customer_Type()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW");
        repo.Add(car);

        var exception = Assert.Throws<InvalidCustomerTypeException>(() =>
            service.RentCar("User", "vip", car.Id, 3));

        Assert.Equal("vip", exception.CustomerType);
        Assert.Contains("Invalid customer type", exception.Message);
    }

    [Fact]
    public void Should_Throw_RentalLimitExceededException_For_Economy_Over_10_Days()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW");
        repo.Add(car);

        var exception = Assert.Throws<RentalLimitExceededException>(() =>
            service.RentCar("User", "economy", car.Id, 15));

        Assert.Equal("economy", exception.CustomerType);
        Assert.Equal(10, exception.MaxDays);
        Assert.Contains("cannot rent for more than 10 days", exception.Message);
    }

    [Fact]
    public void Should_Throw_CarAlreadyRentedException_When_Car_Not_Available()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW");
        repo.Add(car);

        service.RentCar("User1", "economy", car.Id, 3);

        var exception = Assert.Throws<CarAlreadyRentedException>(() =>
            service.RentCar("User2", "economy", car.Id, 2));

        Assert.Equal(car.Id, exception.CarId);
        Assert.Contains("already rented", exception.Message);
    }

    [Fact]
    public void Should_Throw_CarNotFoundException_On_Rent_When_Car_Missing()
    {
        var service = CreateService(out var repo);
        var nonExistentCarId = Guid.NewGuid();

        var exception = Assert.Throws<CarNotFoundException>(() =>
            service.RentCar("User", "economy", nonExistentCarId, 3));

        Assert.Equal(nonExistentCarId, exception.CarId);
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void Should_Throw_RentalNotFoundException_When_No_Active_Rental()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW");
        repo.Add(car);

        var exception = Assert.Throws<RentalNotFoundException>(() =>
            service.ReturnCar(car.Id));

        Assert.Equal(car.Id, exception.CarId);
        Assert.Contains("No active rental found", exception.Message);
    }

    [Fact]
    public void All_Domain_Exceptions_Should_Have_Error_Context()
    {
        var service = CreateService(out var repo);
        var carId = Guid.NewGuid();

        var ex1 = Assert.Throws<CarNotFoundException>(() =>
            service.RentCar("User", "economy", carId, 3));
        Assert.NotEmpty(ex1.Message);

        var car = new Car("Test");
        repo.Add(car);

        var ex2 = Assert.Throws<RentalNotFoundException>(() =>
            service.ReturnCar(car.Id));
        Assert.NotEmpty(ex2.Message);

        var ex3 = Assert.Throws<InvalidCustomerTypeException>(() =>
            service.RentCar("User", "unknown", car.Id, 3));
        Assert.NotEmpty(ex3.Message);
    }
}
