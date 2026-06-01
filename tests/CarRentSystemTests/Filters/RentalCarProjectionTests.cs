using Moq;
using CarRentSystem.Application.Analytics;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Interfaces;
using Xunit;

namespace CarRentSystem.Tests.Analytics;

/// <summary>
/// Unit tests for <see cref="RentalAnalyticsService.GetRentalWithCar"/>
/// and <see cref="RentalAnalyticsService.GetRevenueByBrand"/>.
/// </summary>
public sealed class RentalCarProjectionTests
{
    // ─── GetRentalWithCar ─────────────────────────────────────────────────────

    [Fact]
    public void GetRentalWithCar_SingleMatch_ShouldReturnOneProjection()
    {
        // Arrange
        var car    = MakeCar("Toyota", "Corolla", 80m);
        var rental = MakeRental(car.Id, "Alice", days: 3, pricePerDay: 80m);

        var analytics = BuildAnalytics(
            cars:    new[] { car },
            rentals: new[] { rental });

        // Act
        var result = analytics.GetRentalWithCar().ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(car.Id,       result[0].Car.Id);
        Assert.Equal(rental.Id,    result[0].Rental.Id);
        Assert.Equal("Corolla",    result[0].Car.Model);
        Assert.Equal("Alice",      result[0].Rental.CustomerName);
    }

    [Fact]
    public void GetRentalWithCar_WhenCarIdNotFound_ShouldReturnEmpty()
    {
        // Rental references a car that does not exist in the repository.
        var orphanRental = MakeRental(Guid.NewGuid(), "Bob", days: 2, pricePerDay: 60m);

        var analytics = BuildAnalytics(
            cars:    Array.Empty<Car>(),
            rentals: new[] { orphanRental });

        var result = analytics.GetRentalWithCar().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetRentalWithCar_WhenNoRentals_ShouldReturnEmpty()
    {
        var car = MakeCar("Honda", "Civic", 70m);

        var analytics = BuildAnalytics(
            cars:    new[] { car },
            rentals: Array.Empty<Rental>());

        var result = analytics.GetRentalWithCar().ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void GetRentalWithCar_MultipleRentals_ShouldExcludeOrphans()
    {
        // Two valid rentals + one orphan (no matching car)
        var car1 = MakeCar("Toyota", "Corolla", 80m);
        var car2 = MakeCar("Honda",  "Civic",   70m);

        var rentals = new[]
        {
            MakeRental(car1.Id,     "Alice", days: 3, pricePerDay: 80m),
            MakeRental(car2.Id,     "Bob",   days: 5, pricePerDay: 70m),
            MakeRental(Guid.NewGuid(), "Eve", days: 1, pricePerDay: 50m), // orphan
        };

        var analytics = BuildAnalytics(
            cars:    new[] { car1, car2 },
            rentals: rentals);

        var result = analytics.GetRentalWithCar().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Car.Model == "Corolla");
        Assert.Contains(result, p => p.Car.Model == "Civic");
    }

    [Fact]
    public void GetRentalWithCar_ProjectionShouldContainCorrectTotalPrice()
    {
        var car    = MakeCar("Toyota", "Corolla", 80m);
        var rental = MakeRental(car.Id, "Alice", days: 3, pricePerDay: 80m);

        var analytics = BuildAnalytics(new[] { car }, new[] { rental });

        var projection = analytics.GetRentalWithCar().Single();

        // 3 days * 80 = 240
        Assert.Equal(240m, projection.Rental.TotalPrice);
    }

    // ─── GetRevenueByBrand ────────────────────────────────────────────────────

    [Fact]
    public void GetRevenueByBrand_ShouldGroupByBrandAndSumRevenue()
    {
        var toyota = MakeCar("Toyota", "Corolla", 80m);
        var honda  = MakeCar("Honda",  "Civic",   70m);

        var rentals = new[]
        {
            MakeRental(toyota.Id, "Alice", days: 3, pricePerDay: 80m),  // 240
            MakeRental(toyota.Id, "Bob",   days: 2, pricePerDay: 80m),  // 160  → Toyota total: 400
            MakeRental(honda.Id,  "Eve",   days: 5, pricePerDay: 70m),  // 350  → Honda total: 350
        };

        var analytics = BuildAnalytics(new[] { toyota, honda }, rentals);

        var result = analytics.GetRevenueByBrand().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Toyota", result[0].Key);   // higher revenue first
        Assert.Equal(400m,     result[0].Value);
        Assert.Equal("Honda",  result[1].Key);
        Assert.Equal(350m,     result[1].Value);
    }

    [Fact]
    public void GetRevenueByBrand_WhenNoRentals_ShouldReturnEmpty()
    {
        var car = MakeCar("Toyota", "Corolla", 80m);

        var analytics = BuildAnalytics(new[] { car }, Array.Empty<Rental>());

        Assert.Empty(analytics.GetRevenueByBrand());
    }

    // ─── RentalCarProjection record ───────────────────────────────────────────

    [Fact]
    public void RentalCarProjection_EqualityByValue_ShouldWork()
    {
        var car    = MakeCar("Toyota", "Corolla", 80m);
        var rental = MakeRental(car.Id, "Alice", days: 3, pricePerDay: 80m);

        var p1 = new RentalCarProjection(rental, car);
        var p2 = new RentalCarProjection(rental, car);

        // record uses value equality
        Assert.Equal(p1, p2);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static RentalAnalyticsService BuildAnalytics(
        IEnumerable<Car> cars,
        IEnumerable<Rental> rentals)
    {
        var carRepo    = new Mock<ICarRepository>();
        var rentalRepo = new Mock<IRentalRepository>();

        carRepo.Setup(r => r.GetAll()).Returns(cars.ToList());
        rentalRepo.Setup(r => r.GetAll()).Returns(rentals.ToList());

        // GetById is used by GetTopModels — provide it too
        foreach (var car in cars)
        {
            var c = car;
            carRepo.Setup(r => r.GetById(c.Id)).Returns(c);
        }

        return new RentalAnalyticsService(rentalRepo.Object, carRepo.Object);
    }

    /// <summary>
    /// Creates a Car. Adjust constructor signature if needed.
    /// Car(Guid id, string brand, string model, decimal pricePerDay)
    /// </summary>
    private static Car MakeCar(string brand, string model, decimal pricePerDay) =>
        new Car(Guid.NewGuid(), brand, model, pricePerDay);

    /// <summary>
    /// Creates a Rental with a fixed TotalPrice (days * pricePerDay).
    /// Adjust constructor to match your Rental class signature.
    /// </summary>
    private static Rental MakeRental(
        Guid carId, string customerName, int days, decimal pricePerDay) =>
        new Rental(Guid.NewGuid(), carId, customerName, days, days * pricePerDay);
}