using MyProject.Application.Factories;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;


namespace MyProject.Application.Services;

public class RentalService
{
    private readonly ICarRepository _carRepository;

    public RentalService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public void RentCar(string customerName, Guid carId)
    {
        var car = _carRepository.GetById(carId);

        if (car == null)
            throw new Exception("Car not found");

        var customer = CustomerFactory.Create(customerName, "economy");

        car.Rent();

        var rental = new Rental(car, customer);

        Console.WriteLine($"Customer {customer.Name} rented {car.Model}");
    }
}