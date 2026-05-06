using MyProject.Application.Services;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Repositories;
using Xunit;

public class RentalServiceTests
{
    [Fact]
    public void ShouldRentCar()
    {
        var repo = new InMemoryCarRepository();
        var service = new RentalService(repo);

        var car = new Car("BMW");
        repo.Add(car);

        service.RentCar("User", car.Id);

        Assert.False(car.IsAvailable);
    }
    [Fact]
    public void ShouldThrowIfCarAlreadyRented()
{
    var repo = new InMemoryCarRepository();
    var service = new RentalService(repo);

    var car = new Car("BMW");
    repo.Add(car);

    service.RentCar("User", car.Id);

    Assert.Throws<InvalidOperationException>(() =>
        service.RentCar("User", car.Id));
}

    [Fact]
    public void ShouldHandleMultipleCars()
    {
    var repo = new InMemoryCarRepository();
    var service = new RentalService(repo);

    var car1 = new Car("BMW");
    var car2 = new Car("Audi");

    repo.Add(car1);
    repo.Add(car2);

    service.RentCar("User", car1.Id);

    Assert.False(car1.IsAvailable);
    Assert.True(car2.IsAvailable);
    }
}