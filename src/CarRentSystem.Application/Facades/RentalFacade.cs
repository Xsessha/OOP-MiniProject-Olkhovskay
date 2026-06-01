

using CarRentSystem.Application.Analytics;
using CarRentSystem.Application.Filters;
using CarRentSystem.Application.Services;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Interfaces;

namespace CarRentSystem.Application.Facades;

/// <summary>
/// Facade that exposes a simplified API for the console UI.
/// The UI layer talks only to this class — it does not touch
/// repositories, services, or analytics directly.
/// </summary>
public sealed class RentalFacade
{
    private readonly RentalService _rentalService;
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly RentalAnalyticsService _analytics;

    public RentalFacade(
        RentalService rentalService,
        ICarRepository carRepository,
        IRentalRepository rentalRepository,
        RentalAnalyticsService? analytics = null)
    {
        ArgumentNullException.ThrowIfNull(rentalService);
        ArgumentNullException.ThrowIfNull(carRepository);
        ArgumentNullException.ThrowIfNull(rentalRepository);

        _rentalService = rentalService;
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;

        _analytics = analytics ?? new RentalAnalyticsService(rentalRepository, carRepository);
    }

 
    public IReadOnlyList<Car> GetAllCars() =>
        _carRepository.GetAll().ToList();

    
    public RentOperationResult RentCar(string customerName, string customerType, Guid carId, int days) =>
        _rentalService.RentCar(customerName, customerType, carId, days);

   
    public ReturnOperationResult ReturnCar(Guid carId) =>
        _rentalService.ReturnCar(carId);

    public decimal GetTotalRevenue() =>
        _analytics.GetTotalRevenue();

    public decimal GetRevenue() =>
        _analytics.GetTotalRevenue();

    public IEnumerable<KeyValuePair<string, int>> GetTopModels(int top = 3) =>
        _analytics.GetTopModels(top);

    public IEnumerable<Rental> SearchByCustomer(string name) =>
        _analytics.SearchByCustomer(name);

 
    public IEnumerable<Car> SearchCars(CarFilterPipeline pipeline)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        return _carRepository.GetAll().Where(pipeline.Matches);
    }


    public IEnumerable<RentalCarProjection> GetRentalWithCar() =>
        _analytics.GetRentalWithCar();


    public IEnumerable<KeyValuePair<string, decimal>> GetRevenueByBrand() =>
        _analytics.GetRevenueByBrand();

    public List<Car> GetAvailableCars() =>
        _carRepository.GetAll().Where(c => c.IsAvailable).ToList();

    public List<Car> GetCars() =>
        _carRepository.GetAll().ToList();

    public RentOperationResult Rent(string customerName, string customerType, Guid carId, int days) =>
        RentCar(customerName, customerType, carId, days);

    public ReturnOperationResult Return(Guid carId) =>
        ReturnCar(carId);

    public RentalAnalyticsReport GetAnalyticsReport(RentalQuery? query = null)
    {
        var q = query ?? RentalQuery.All;
        var all = _rentalRepository.GetAll();
        var rentals = q.Apply(all).ToList();

        var totalRevenue = rentals.Sum(r => r.TotalPrice);
        var rentalCount = rentals.Count;
        var activeRentalCount = rentals.Count(r => !r.Car.IsAvailable);
        var avgDays = rentals.Any() ? rentals.Average(r => r.Days) : 0.0;

        var modelStats = rentals
            .GroupBy(r => r.Car.Model, StringComparer.OrdinalIgnoreCase)
            .Select(g => new RentalModelStatistic(
                g.Key,
                g.Count(),
                g.Sum(r => r.TotalPrice)))
            .OrderByDescending(s => s.Revenue)
            .ToList();

        return new RentalAnalyticsReport(rentalCount, activeRentalCount, totalRevenue, avgDays, modelStats);
    }
}