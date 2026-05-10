using Xunit;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Repositories;

namespace MyProject.Tests.Application;

public class RepositoryTests
{
    [Fact]
    public void Empty_Repository_Should_Return_Empty_List()
    {
        var repo = new InMemoryCarRepository();

        var cars = repo.GetAll();

        Assert.Empty(cars);
    }

    [Fact]
    public void Repository_Should_Add_Car()
    {
        var repo = new InMemoryCarRepository();

        var car = new Car("BMW");

        repo.Add(car);

        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Repository_Should_Return_Car_By_Id()
    {
        var repo = new InMemoryCarRepository();

        var car = new Car("Audi");

        repo.Add(car);

        var found = repo.GetById(car.Id);

        Assert.Equal(car.Id, found!.Id);
    }

    [Fact]
    public void Repository_Should_Return_Null_For_Missing_Car()
    {
        var repo = new InMemoryCarRepository();

        var car = repo.GetById(Guid.NewGuid());

        Assert.Null(car);
    }

    [Fact]
    public void Repository_Should_Handle_Multiple_Cars()
    {
        var repo = new InMemoryCarRepository();

        repo.Add(new Car("BMW"));
        repo.Add(new Car("Audi"));
        repo.Add(new Car("Tesla"));

        Assert.Equal(3, repo.GetAll().Count);
    }
}