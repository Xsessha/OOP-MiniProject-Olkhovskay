using Xunit;
using CarRentSystem.Tests.Fixtures;

namespace CarRentSystem.Tests.Domain;

public class FixtureUsageTests
{
    [Fact]
    public void Fixture_Should_Create_Car()
    {
        var car = TestDataFixture.CreateCar();

        Assert.Equal("BMW", car.Model);
    }

    [Fact]
    public void Fixture_Should_Create_Rental()
    {
        var rental = TestDataFixture.CreateRental();

        Assert.NotNull(rental);
    }

    [Fact]
    public void Fixture_Should_Create_PremiumCustomer()
    {
        var customer = TestDataFixture.CreatePremiumCustomer();

        Assert.True(customer.GetDiscount() > 0);
    }
}