using Xunit;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;

namespace MyProject.Tests.Domain;

public class StateTransitionTests
{
    [Fact]
    public void Car_Should_Change_State_To_Rented()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.False(car.IsAvailable);
    }

    [Fact]
    public void Car_Should_Change_State_Back_To_Available()
    {
        var car = new Car("BMW");

        car.Rent();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Should_Not_Allow_Double_Rent()
    {
        var car = new Car("BMW");

        car.Rent();

        Assert.Throws<CarAlreadyRentedException>(() =>
            car.Rent());
    }

    [Fact]
    public void Double_Return_Should_Not_Crash()
    {
        var car = new Car("BMW");

        car.Return();
        car.Return();

        Assert.True(car.IsAvailable);
    }
}