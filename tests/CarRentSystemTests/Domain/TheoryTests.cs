using Xunit;
using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Tests.Domain;

public class TheoryTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Customer_Should_Reject_Invalid_Name(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new EconomyCustomer(name!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Rental_Should_Reject_Invalid_Days(int days)
    {
        var car = new Car("BMW");
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() =>
            new Rental(car, customer, days));
    }

    [Theory]
    [InlineData("BMW")]
    [InlineData("Audi")]
    [InlineData("Tesla")]
    public void Car_Should_Store_Model(string model)
    {
        var car = new Car(model);

        Assert.Equal(model, car.Model);
    }
}