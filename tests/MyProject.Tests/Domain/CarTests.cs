using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
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

        Assert.Throws<CarAlreadyRentedException>(() => car.Rent());
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
[Fact]
    public void Should_Rent_Car()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void Should_Return_Car_And_Be_Available()
    {
        var car = new Car("BMW");

        car.Rent();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Should_Not_Rent_Twice()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.Throws<CarAlreadyRentedException>(() => car.Rent());
    }

    [Fact]
    public void Should_Not_Crash_On_Double_Return()
    {
        var car = new Car("BMW");

        car.Return();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Id_Should_Be_Unique()
    {
        var c1 = new Car("BMW");
        var c2 = new Car("BMW");

        Assert.NotEqual(c1.Id, c2.Id);
    }

    [Fact]
    public void Car_Model_Should_Stay_Same()
    {
        var car = new Car("Tesla");

        Assert.Equal("Tesla", car.Model);
    }

    [Fact]
    public void Rent_Should_Change_State()
    {
        var car = new Car("Audi");

        car.Rent();

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void Return_Should_Restore_State()
    {
        var car = new Car("Audi");

        car.Rent();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Multiple_Cars_Should_Not_Interfere()
    {
        var c1 = new Car("BMW");
        var c2 = new Car("Audi");

        c1.Rent();

        Assert.False(c1.IsAvailable);
        Assert.True(c2.IsAvailable);
    }
}
