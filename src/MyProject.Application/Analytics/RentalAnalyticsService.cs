using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Analytics;

public class RentalAnalyticsService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public RentalAnalyticsService(IRentalRepository rentalRepository, ICarRepository carRepository)
    {
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    public List<Rental> GetActiveRentals()
    {
        return _rentalRepository.GetAll()
            .Where(r => r.Car.IsAvailable == false)
            .ToList();
    }

    public List<Rental> SearchByCustomer(string name)
    {
        return _rentalRepository.GetAll()
            .Where(r => r.Customer.Name.ToLower().Contains(name.ToLower()))
            .ToList();
    }

    public List<Car> GetTopRentedCars()
    {
        return _rentalRepository.GetAll()
            .GroupBy(r => r.Car.Id)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.First().Car)
            .ToList();
    }

    public List<Car> GetCarsSortedByPrice()
    {
        return _carRepository.GetAll()
            .OrderByDescending(c => c.PricePerDay)
            .ToList();
    }

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

    public HashSet<string> GetUniqueCustomers()
    {
        return _rentalRepository.GetAll()
            .Select(r => r.Customer.Name)
            .ToHashSet();
    }

    public decimal GetTotalRevenue()
    {
        return _rentalRepository.GetAll()
            .Sum(r => r.TotalPrice);
    }
}