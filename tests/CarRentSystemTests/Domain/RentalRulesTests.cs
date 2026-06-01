using Xunit;
using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Tests.Domain;

public class RentalRulesTests
{
    [Fact]
    public void Car_Should_Have_Price()
    {
        var car = new Car("BMW X5");

        Assert.True(car.PricePerDay > 0);
    }

    [Fact]
    public void Premium_Should_Have_Bigger_Discount()
    {
        var p = new PremiumCustomer("Alex");
        var e = new EconomyCustomer("Bob");

        Assert.True(p.GetDiscount() > e.GetDiscount());
    }

    [Fact]
    public void Rental_Should_Calculate_Price()
    {
        var car = new Car("BMW X5");
        var customer = new EconomyCustomer("John");

        var rental = new Rental(car, customer, 3);

        Assert.True(rental.TotalPrice > 0);
    }

    [Fact]
    public void Premium_Should_Pay_Less()
    {
        var car = new Car("BMW X5");

        var eco = new EconomyCustomer("A");
        var prem = new PremiumCustomer("B");

        var r1 = new Rental(car, eco, 3);
        var r2 = new Rental(car, prem, 3);

        Assert.True(r2.TotalPrice < r1.TotalPrice);
    }
}