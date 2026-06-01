using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Interfaces;
using CarRentSystem.Application.Caching;

namespace CarRentSystem.Application.Analytics;
public sealed class RentalAnalyticsService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICarRepository _carRepository;

    public RentalAnalyticsService(
        IRentalRepository rentalRepository,
        ICarRepository carRepository)
    {
        ArgumentNullException.ThrowIfNull(rentalRepository);
        ArgumentNullException.ThrowIfNull(carRepository);
        _rentalRepository = rentalRepository;
        _carRepository = carRepository;
    }

    public decimal GetTotalRevenue() =>
        _rentalRepository.GetAll().Sum(r => r.TotalPrice);

    public IEnumerable<KeyValuePair<string, int>> GetTopModels(int top = 3)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var rental in _rentalRepository.GetAll())
        {
            var car = _carRepository.GetById(rental.CarId);
            if (car is null) continue;

            if (!counts.TryAdd(car.Model, 1))
                counts[car.Model]++;
        }

        return counts
            .OrderByDescending(kv => kv.Value)
            .Take(top);
    }

  
    public IEnumerable<Rental> SearchByCustomer(string customerName) =>
        _rentalRepository.GetAll()
            .Where(r => r.CustomerName.Contains(
                customerName, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyCollection<string> GetUniqueCustomers()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rental in _rentalRepository.GetAll())
            names.Add(rental.CustomerName);
        return names;
    }

    
    public IEnumerable<RentalCarProjection> GetRentalWithCar() =>
        from rental in _rentalRepository.GetAll()
        join car in _carRepository.GetAll()
            on rental.CarId equals car.Id
        select new RentalCarProjection(rental, car);

    public IReadOnlyList<Rental> GetActiveRentals()
    {
        return _rentalRepository.GetAll().Where(r => !r.Car.IsAvailable).ToList();
    }

    public List<Car> GetTopRentedCars(int top = 5)
    {
        var topModels = _rentalRepository.GetAll()
            .GroupBy(r => r.Car.Model, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Take(top)
            .Select(g => g.Key)
            .ToList();

        var cars = _carRepository.GetAll()
            .Where(c => topModels.Contains(c.Model))
            .OrderByDescending(c => topModels.IndexOf(c.Model))
            .ToList();

        return cars;
    }

    public IReadOnlyList<Car> GetCarsSortedByPrice()
    {
        return _carRepository.GetAll().OrderByDescending(c => c.PricePerDay).ToList();
    }

    public IReadOnlyDictionary<string, int> GetCarPopularity()
    {
        return _rentalRepository.GetAll()
            .GroupBy(r => r.Car.Model)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);
    }

    
    public IEnumerable<KeyValuePair<string, decimal>> GetRevenueByBrand() =>
        GetRentalWithCar()
            .GroupBy(p => p.Car.Brand, StringComparer.OrdinalIgnoreCase)
            .Select(g => new KeyValuePair<string, decimal>(
                g.Key,
                g.Sum(p => p.Rental.TotalPrice)))
            .OrderByDescending(kv => kv.Value);
}