using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Analytics;

/// <summary>
/// Provides read-only analytical queries over cars and rentals.
/// </summary>
public class RentalAnalyticsService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public RentalAnalyticsService(IRentalRepository rentalRepository, ICarRepository carRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    /// <summary>
    /// Returns rentals whose cars are still marked as unavailable.
    /// </summary>
    public List<Rental> GetActiveRentals()
    {
        return _rentalRepository.GetAll()
            .Where(r => r.Car.IsAvailable == false)
            .ToList();
    }

    /// <summary>
    /// Finds rentals by customer name using case-insensitive substring matching.
    /// </summary>
    public List<Rental> SearchByCustomer(string name)
    {
        return _rentalRepository.GetAll()
            .Where(r => r.Customer.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Returns up to five cars ordered by number of rentals.
    /// </summary>
    public List<Car> GetTopRentedCars()
    {
        return _rentalRepository.GetAll()
            .GroupBy(r => r.Car.Id)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.First().Car)
            .ToList();
    }

    /// <summary>
    /// Returns all cars ordered from the most expensive to the cheapest.
    /// </summary>
    public List<Car> GetCarsSortedByPrice()
    {
        return _carRepository.GetAll()
            .OrderByDescending(c => c.PricePerDay)
            .ToList();
    }

    /// <summary>
    /// Counts how many rentals each car model has.
    /// </summary>
    public Dictionary<string, int> GetCarPopularity()
    {
        var dict = new Dictionary<string, int>();

        foreach (var rental in _rentalRepository.GetAll())
        {
            if (dict.ContainsKey(rental.Car.Model))
                dict[rental.Car.Model]++;
            else
                dict[rental.Car.Model] = 1;
        }

        return dict;
    }

    /// <summary>
    /// Returns unique customer names with O(1) average membership checks.
    /// </summary>
    public HashSet<string> GetUniqueCustomers()
    {
        return _rentalRepository.GetAll()
            .Select(r => r.Customer.Name)
            .ToHashSet();
    }

    /// <summary>
    /// Aggregates revenue across all recorded rentals.
    /// </summary>
    public decimal GetTotalRevenue()
    {
        return _rentalRepository.GetAll()
            .Sum(r => r.TotalPrice);
    }
}
