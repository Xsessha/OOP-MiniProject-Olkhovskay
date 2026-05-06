using MyProject.Domain.Entities;
using Xunit;

public class CarTests
{
    [Fact]
    public void ShouldRentCar()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void ShouldNotAllowDoubleRent()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.Throws<InvalidOperationException>(() => car.Rent());
    }
    [Fact]
public void ShouldBeAvailableAfterCreation()
{
    var car = new Car("BMW");

    Assert.True(car.IsAvailable);
}

[Fact]
public void ShouldReturnCarEvenIfNotRented()
{
    var car = new Car("Audi");

    car.Return();

    Assert.True(car.IsAvailable);
}

[Fact]
public void ShouldHaveUniqueId()
{
    var car1 = new Car("BMW");
    var car2 = new Car("BMW");

    Assert.NotEqual(car1.Id, car2.Id);
}
}
