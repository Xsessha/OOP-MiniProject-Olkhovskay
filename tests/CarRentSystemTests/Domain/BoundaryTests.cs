using Xunit;
using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Tests.Domain;

public class BoundaryTests
{
    [Fact]
    public void Should_Reject_Too_Long_Rental()
    {
        var car = new Car("BMW");
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() =>
            new Rental(car, customer, 9999));
    }

    [Fact]
    public void Should_Reject_Negative_Rental_Days()
    {
        var car = new Car("BMW");
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() =>
            new Rental(car, customer, -100));
    }

    [Fact]
    public void Should_Reject_Zero_Rental_Days()
    {
        var car = new Car("BMW");
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() =>
            new Rental(car, customer, 0));
    }

    [Fact]
    public void Price_Should_Always_Be_Positive()
    {
        var car = new Car("BMW");

        Assert.True(car.PricePerDay > 0);
    }
}