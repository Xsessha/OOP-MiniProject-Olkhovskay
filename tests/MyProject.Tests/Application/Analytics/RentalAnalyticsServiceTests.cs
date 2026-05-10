using MyProject.Application.Analytics;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Repositories;

namespace MyProject.Tests.Application.Analytics;

public class RentalAnalyticsServiceTests
{
    private static RentalAnalyticsService CreateService(
        IEnumerable<Car>? cars = null,
        IEnumerable<Rental>? rentals = null)
    {
        var carRepository = new InMemoryCarRepository(cars ?? Enumerable.Empty<Car>());
        var rentalRepository = new InMemoryRentalRepository();

        foreach (var rental in rentals ?? Enumerable.Empty<Rental>())
            rentalRepository.Add(rental);

        return new RentalAnalyticsService(rentalRepository, carRepository);
    }

    [Fact]
    public void GetActiveRentals_Should_Return_Only_Rentals_With_Unavailable_Cars()
    {
        var activeCar = new Car("BMW X5");
        var returnedCar = new Car("Audi A6");
        activeCar.Rent();

        var activeRental = CreateRental(activeCar, "Alice");
        var returnedRental = CreateRental(returnedCar, "Bob");
        var service = CreateService(rentals: new[] { activeRental, returnedRental });

        var result = service.GetActiveRentals();

        Assert.Single(result);
        Assert.Same(activeRental, result[0]);
    }

    [Theory]
    [InlineData("ali", 2)]
    [InlineData("ALICE", 1)]
    [InlineData("bob", 1)]
    [InlineData("missing", 0)]
    public void SearchByCustomer_Should_Search_Case_Insensitively(string query, int expectedCount)
    {
        var rentals = new[]
        {
            CreateRental(new Car("BMW X5"), "Alice"),
            CreateRental(new Car("Audi A6"), "Alicia"),
            CreateRental(new Car("Toyota Camry"), "Bob")
        };
        var service = CreateService(rentals: rentals);

        var result = service.SearchByCustomer(query);

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void GetTopRentedCars_Should_Order_By_Rental_Count_And_Limit_To_Five()
    {
        var cars = Enumerable.Range(1, 6)
            .Select(i => new Car(Guid.NewGuid(), $"Model {i}", true, 50 + i))
            .ToArray();

        var rentals = new List<Rental>();
        for (var i = 0; i < cars.Length; i++)
        {
            for (var count = 0; count < i + 1; count++)
                rentals.Add(CreateRental(cars[i], $"Customer {i}-{count}"));
        }

        var service = CreateService(rentals: rentals);

        var result = service.GetTopRentedCars();

        Assert.Equal(5, result.Count);
        Assert.Equal("Model 6", result[0].Model);
        Assert.DoesNotContain(result, car => car.Model == "Model 1");
    }

    [Fact]
    public void GetCarsSortedByPrice_Should_Return_Cars_In_Descending_Price_Order()
    {
        var cheap = new Car("Cheap", 40);
        var expensive = new Car("Expensive", 200);
        var middle = new Car("Middle", 90);
        var service = CreateService(cars: new[] { cheap, expensive, middle });

        var result = service.GetCarsSortedByPrice();

        Assert.Equal(new[] { "Expensive", "Middle", "Cheap" }, result.Select(c => c.Model));
    }

    [Fact]
    public void GetCarPopularity_Should_Count_Rentals_Per_Model()
    {
        var rentals = new[]
        {
            CreateRental(new Car("BMW X5"), "Alice"),
            CreateRental(new Car("BMW X5"), "Bob"),
            CreateRental(new Car("Audi A6"), "Charlie")
        };
        var service = CreateService(rentals: rentals);

        var popularity = service.GetCarPopularity();

        Assert.Equal(2, popularity["BMW X5"]);
        Assert.Equal(1, popularity["Audi A6"]);
    }

    [Fact]
    public void GetUniqueCustomers_Should_Remove_Duplicates()
    {
        var rentals = new[]
        {
            CreateRental(new Car("BMW X5"), "Alice"),
            CreateRental(new Car("Audi A6"), "Alice"),
            CreateRental(new Car("Toyota Camry"), "Bob")
        };
        var service = CreateService(rentals: rentals);

        var customers = service.GetUniqueCustomers();

        Assert.Equal(2, customers.Count);
        Assert.Contains("Alice", customers);
        Assert.Contains("Bob", customers);
    }

    [Fact]
    public void GetTotalRevenue_Should_Return_Zero_For_No_Rentals()
    {
        var service = CreateService();

        Assert.Equal(0, service.GetTotalRevenue());
    }

    [Fact]
    public void GetTotalRevenue_Should_Sum_All_Rental_Prices()
    {
        var rentals = new[]
        {
            CreateRental(new Car("BMW X5", 100), "Alice", days: 2),
            CreateRental(new Car("Audi A6", 80), "Bob", days: 3)
        };
        var service = CreateService(rentals: rentals);

        var result = service.GetTotalRevenue();

        Assert.Equal(rentals.Sum(r => r.TotalPrice), result);
    }

    private static Rental CreateRental(Car car, string customerName, int days = 1)
    {
        return new Rental(car, new EconomyCustomer(customerName), days);
    }
}
