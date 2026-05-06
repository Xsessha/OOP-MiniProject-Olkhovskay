using MyProject.Domain.Entities;
using Xunit;

public class RentalTests
{
    [Fact]
    public void ShouldCreateRental()
    {
        var car = new Car("BMW");
        var customer = new EconomyCustomer("John");

        var rental = new Rental(car, customer);

        Assert.NotNull(rental);
    }

    [Fact]
    public void ShouldThrowIfCarNull()
    {
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentNullException>(() =>
            new Rental(null!, customer));
    }

    [Fact]
    public void ShouldThrowIfCustomerNull()
    {
        var car = new Car("BMW");

        Assert.Throws<ArgumentNullException>(() =>
            new Rental(car, null!));
    }
}