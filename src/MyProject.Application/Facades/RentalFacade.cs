using MyProject.Application.Services;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Facades;

public class RentalFacade
{
    private readonly RentalService _service;
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;

    public RentalFacade(
        RentalService service,
        ICarRepository carRepository,
        IRentalRepository rentalRepository)
    {
        _service = service;
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
    }

    public void Rent(string name, string type, Guid carId, int days)
    {
        _service.RentCar(name, type, carId, days);
    }

    public void Return(Guid carId)
    {
        _service.ReturnCar(carId);
    }

    public List<Car> GetCars()
    {
        return _carRepository.GetAll();
    }

    public List<Car> GetAvailableCars()
    {
        return _carRepository.GetAll()
            .Where(c => c.IsAvailable)
            .ToList();
    }

    public decimal GetRevenue()
    {
        return _rentalRepository.GetAll()
            .Sum(r => r.TotalPrice);
    }

    public IEnumerable<IGrouping<string, Rental>> GetTopCars()
    {
        return _rentalRepository.GetAll()
            .GroupBy(r => r.Car.Model)
            .OrderByDescending(g => g.Count());
    }
}