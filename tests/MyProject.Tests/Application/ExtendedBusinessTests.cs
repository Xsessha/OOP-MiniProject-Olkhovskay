using Xunit;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using MyProject.Application.Services;
using MyProject.Infrastructure.Repositories;
using MyProject.Application.Facades;
using System;
using System.Linq;

namespace MyProject.Tests.Application;

public class ExtendedBusinessTests
{
    private RentalService CreateService(out InMemoryCarRepository carRepo, out InMemoryRentalRepository rentalRepo)
    {
        carRepo = new InMemoryCarRepository();
        rentalRepo = new InMemoryRentalRepository();
        return new RentalService(carRepo, rentalRepo);
    }

    // ===================== CAR RULES =====================

    [Fact]
    public void Car_Should_Always_Have_Positive_Price()
    {
        var car = new Car("BMW", 100);

        Assert.True(car.PricePerDay > 0);
    }

    [Fact]
    public void Rent_Return_Flow_Should_Be_Stable()
    {
        var car = new Car("Audi", 90);

        car.Rent();
        car.Return();
        car.Rent();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    // ===================== RENTAL RULES =====================

    [Fact]
    public void Rental_Should_Calculate_TotalPrice()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        var rental = new Rental(car, customer, 3);

        Assert.True(rental.TotalPrice > 0);
    }

    [Fact]
    public void Premium_Should_Pay_Less_Than_Economy()
    {
        var car = new Car("BMW", 100);

        var eco = new EconomyCustomer("A");
        var prem = new PremiumCustomer("B");

        var r1 = new Rental(car, eco, 3);
        var r2 = new Rental(car, prem, 3);

        Assert.True(r2.TotalPrice < r1.TotalPrice);
    }

    [Fact]
    public void Rental_Days_Should_Affect_Price()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        var r1 = new Rental(car, customer, 2);
        var r2 = new Rental(car, customer, 5);

        Assert.True(r2.TotalPrice > r1.TotalPrice);
    }

    // ===================== SERVICE =====================

    [Fact]
    public void Service_Should_Prevent_Double_Rent()
    {
        var service = CreateService(out var repo, out _);

        var car = new Car("BMW", 100);
        repo.Add(car);

        service.RentCar("User", "economy", car.Id, 2);

        Assert.Throws<CarAlreadyRentedException>(() =>
            service.RentCar("User", "economy", car.Id, 2));
    }

    [Fact]
    public void Service_Should_Handle_Return_Without_Crash()
    {
        var service = CreateService(out var repo, out _);

        var car = new Car("BMW", 100);
        repo.Add(car);

        service.RentCar("User", "economy", car.Id, 2);
        service.ReturnCar(car.Id);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Service_Should_Not_Find_Invalid_Car()
    {
        var service = CreateService(out _, out _);

        Assert.Throws<CarNotFoundException>(() =>
            service.RentCar("User", "economy", Guid.NewGuid(), 1));
    }

    // ===================== FACADE =====================

    [Fact]
    public void Facade_Should_Return_All_Cars()
    {
        var carRepo = new InMemoryCarRepository();
        var rentalRepo = new InMemoryRentalRepository();
        var service = new RentalService(carRepo, rentalRepo);

        var facade = new RentalFacade(service, carRepo, rentalRepo);

        carRepo.Add(new Car("BMW", 100));

        var cars = facade.GetCars();

        Assert.Single(cars);
    }

    [Fact]
    public void Facade_Should_Return_Available_Only()
    {
        var carRepo = new InMemoryCarRepository();
        var rentalRepo = new InMemoryRentalRepository();
        var service = new RentalService(carRepo, rentalRepo);

        var facade = new RentalFacade(service, carRepo, rentalRepo);

        var car = new Car("BMW", 100);
        carRepo.Add(car);

        var available = facade.GetAvailableCars();

        Assert.Contains(car, available);
    }

    [Fact]
    public void Facade_Revenue_Should_Be_Zero_If_No_Rentals()
    {
        var carRepo = new InMemoryCarRepository();
        var rentalRepo = new InMemoryRentalRepository();
        var service = new RentalService(carRepo, rentalRepo);

        var facade = new RentalFacade(service, carRepo, rentalRepo);

        Assert.Equal(0, facade.GetRevenue());
    }

    // ===================== COLLECTION RULES =====================

    [Fact]
    public void Cars_Should_Not_Overlap_Between_Instances()
    {
        var c1 = new Car("BMW", 100);
        var c2 = new Car("Audi", 100);

        c1.Rent();

        Assert.False(c1.IsAvailable);
        Assert.True(c2.IsAvailable);
    }

    [Fact]
    public void Multiple_Rentals_Should_Work_Correctly()
    {
        var car = new Car("BMW", 100);

        for (int i = 0; i < 5; i++)
        {
            car.Rent();
            car.Return();
        }

        Assert.True(car.IsAvailable);
    }


    [Fact]
    public void Returning_Already_Available_Car_Should_Not_Crash()
    {
        var car = new Car("BMW", 100);

        car.Return();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Customer_Discount_Should_Be_Stable()
    {
        var c = new PremiumCustomer("Alex");

        var d1 = c.GetDiscount();
        var d2 = c.GetDiscount();

        Assert.Equal(d1, d2);
    }
}