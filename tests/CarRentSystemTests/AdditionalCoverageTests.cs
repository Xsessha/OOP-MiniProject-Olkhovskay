using System;
using System.Linq;
using CarRentSystem.Application.Services;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace CarRentSystem.Tests
{
    public class AdditionalCoverageTests
    {
        [Fact]
        public void GetAvailableCars_ReturnsOnlyAvailable()
        {
            var car1 = new Car("Toyota Corolla", 50m);
            var car2 = new Car("Honda Civic", 60m);
            // mark one as rented
            car2.Rent();

            var carRepo = new InMemoryCarRepository(new[] { car1, car2 });
            var rentalRepo = new InMemoryRentalRepository();

            var service = new RentalService(carRepo, rentalRepo, new CarRentSystem.Application.Services.SystemDateTimeProvider());

            var available = service.GetAvailableCars();

            Assert.Single(available);
            Assert.Contains(available, c => c.Id == car1.Id);
        }

        [Fact]
        public void ReturnCar_CalculatesPenalty_WhenLate()
        {
            var car = new Car("Toyota Prius", 100m);
            var customer = new PremiumCustomer("Alice");

            var rental = new Rental(car, customer, 1);

            var carRepo = new InMemoryCarRepository(new[] { car });
            var rentalRepo = new InMemoryRentalRepository();
            rentalRepo.Add(rental);

            var mockDate = new Mock<CarRentSystem.Application.Services.IDateTimeProvider>();
            // make current time after expected return date by 2 days
            mockDate.Setup(d => d.Now).Returns(rental.RentedAt.AddDays(rental.Days + 2));

            var service = new RentalService(carRepo, carRepo, rentalRepo, rentalRepo, mockDate.Object);

            var result = service.ReturnCar(car.Id);

            Assert.True(result.Penalty > 0);
            Assert.Equal(rental, result.Rental);
            Assert.Equal(result.TotalCost, rental.TotalPrice + result.Penalty);
            Assert.True(result.IsLate);
        }

        [Fact]
        public void ReturnOperationResult_Getters_Work()
        {
            var car = new Car("Hyundai Tucson", 40m);
            var customer = new EconomyCustomer("Bob");
            var rental = new Rental(car, customer, 2);

            var res = new ReturnOperationResult(rental, 15m);

            Assert.Equal(rental, res.Rental);
            Assert.Equal(15m, res.Penalty);
            Assert.Equal(rental.TotalPrice + 15m, res.TotalCost);
            Assert.True(res.IsLate);
        }
    }
}
