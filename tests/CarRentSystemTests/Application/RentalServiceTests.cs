using Xunit;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Exceptions;
using CarRentSystem.Infrastructure.Repositories;
using CarRentSystem.Application.Services;

namespace CarRentSystem.Tests.Application;

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

    [Fact]
    public void Should_Use_Customer_Discount_For_Discounted_Price()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW", 100);
        repo.Add(car);

        var result = service.RentCar("User", "economy", car.Id, 3);

        Assert.Equal(300, result.BasePrice);
        Assert.Equal(285, result.DiscountedPrice);
        Assert.Equal(result.Rental.TotalPrice, result.DiscountedPrice);
    }

    [Fact]
    public void Should_Accept_Customer_Type_Case_Insensitively()
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW", 100);
        repo.Add(car);

        var result = service.RentCar("User", " Premium ", car.Id, 2);

        Assert.Equal("premium", result.CustomerType);
        Assert.Equal(160, result.DiscountedPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(366)]
    public void Should_Not_Mutate_Car_When_Rental_Days_Are_Invalid(int days)
    {
        var service = CreateService(out var repo);
        var car = new Car("BMW", 100);
        repo.Add(car);

        Assert.Throws<ArgumentException>(() =>
            service.RentCar("User", "economy", car.Id, days));

        Assert.True(car.IsAvailable);
    }
}
