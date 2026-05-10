using Xunit;
using MyProject.Domain.Entities;

namespace MyProject.Tests.Domain;

public class EdgeCaseTests
{
    [Fact]
    public void Multiple_Returns_Should_Not_Crash()
    {
        var car = new Car("BMW");

        car.Return();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Multiple_Rent_Return_Cycles()
    {
        var car = new Car("Audi");

        for (int i = 0; i < 3; i++)
        {
            car.Rent();
            car.Return();
        }

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Different_Cars_Should_Be_Independent()
    {
        var c1 = new Car("BMW");
        var c2 = new Car("Audi");

        c1.Rent();

        Assert.False(c1.IsAvailable);
        Assert.True(c2.IsAvailable);
    }
}