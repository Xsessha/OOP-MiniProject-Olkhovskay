using MyProject.Domain.Entities;
using MyProject.Domain.ValueObjects;

namespace MyProject.Tests.Domain;

public class FuzzLikeDomainTests
{
    [Fact]
    public void Money_Should_Preserve_NonNegative_Amounts_For_Deterministic_Sample()
    {
        var random = new Random(36);

        for (var i = 0; i < 100; i++)
        {
            var amount = Math.Round((decimal)random.NextDouble() * 10_000m, 2);

            var money = new Money(amount);

            Assert.Equal(amount, money.Amount);
        }
    }

    [Fact]
    public void Rental_TotalPrice_Should_Never_Exceed_BasePrice_For_Known_Customers()
    {
        var random = new Random(36);

        for (var i = 0; i < 50; i++)
        {
            var price = random.Next(1, 500);
            var days = random.Next(1, 31);
            var car = new Car($"Generated {i}", price);

            var economyRental = new Rental(car, new EconomyCustomer($"Economy {i}"), days);
            var premiumRental = new Rental(car, new PremiumCustomer($"Premium {i}"), days);
            var basePrice = price * days;

            Assert.InRange(economyRental.TotalPrice, 0, basePrice);
            Assert.InRange(premiumRental.TotalPrice, 0, basePrice);
            Assert.True(premiumRental.TotalPrice < economyRental.TotalPrice);
        }
    }
}
