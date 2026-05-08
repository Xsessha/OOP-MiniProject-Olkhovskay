using MyProject.Application.Factories;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Services;

public class RentalService
{
    private readonly ICarRepository _carRepository;
    private readonly IRentalRepository _rentalRepository;

    public RentalService(ICarRepository carRepository, IRentalRepository rentalRepository)
    {
        _carRepository = carRepository;
        _rentalRepository = rentalRepository;
    }

    // ===================== USE CASE 1: RENT =====================
    public Rental RentCar(string customerName, string customerType, Guid carId, int days)
    {
        ValidateCustomerLimit(customerType, days);

        var car = _carRepository.GetById(carId);

        if (car == null)
            throw new CarNotFoundException(carId);

        car.Rent();
        _carRepository.Update(car);

        var customer = CustomerFactory.Create(customerName, customerType);

        var basePrice = car.PricePerDay * days;
        var finalPrice = ApplyDiscount(basePrice, customerType);

        var rental = new Rental(car, customer, days);

        _rentalRepository.Add(rental);

        Console.WriteLine($"Customer: {customer.Name} ({customerType})");
        Console.WriteLine($"Car: {car.Model}");
        Console.WriteLine($"Base price: {basePrice:C2}");
        Console.WriteLine($"Final price after discount: {finalPrice:C2}");

        return rental;
    }

    // ===================== USE CASE 2: RETURN =====================
    public void ReturnCar(Guid carId)
    {
        var car = _carRepository.GetById(carId);

        if (car == null)
            throw new Exception("Car not found");

        var rental = _rentalRepository.GetAll()
            .FirstOrDefault(r => r.Car.Id == carId);

        if (rental == null)
            throw new RentalNotFoundException(carId);

        car.Return();
        _carRepository.Update(car);

        var penalty = CalculatePenalty(rental);

        Console.WriteLine($"Car returned: {car.Model}");

        if (penalty > 0)
        {
            Console.WriteLine($"Late penalty: {penalty:C2}");
        }
        else
        {
            Console.WriteLine("Returned on time. No penalty.");
        }

        Console.WriteLine($"Total cost: {(rental.TotalPrice + penalty):C2}");

        _rentalRepository.GetAll().Remove(rental);
    }

    // ===================== USE CASE 3: RULES =====================

    private decimal ApplyDiscount(decimal price, string type)
    {
        return type switch
        {
            "premium" => price * 0.8m,
            "economy" => price,
            _ => price
        };
    }

    private void ValidateCustomerLimit(string type, int days)
    {
        if (type != "economy" && type != "premium")
            throw new InvalidCustomerTypeException(type);

        if (type == "economy" && days > 10)
            throw new RentalLimitExceededException(type, 10);

        if (type == "premium" && days > 30)
            Console.WriteLine("Premium long-term rental approved");
    }

    private decimal CalculatePenalty(Rental rental)
    {
        var expected = rental.RentedAt.AddDays(rental.Days);

        if (DateTime.Now <= expected)
            return 0;

        var lateDays = (DateTime.Now - expected).Days;

        return lateDays * rental.Car.PricePerDay * 1.5m;
    }

    public List<Car> GetAvailableCars()
    {
        return _carRepository.GetAll()
            .Where(c => c.IsAvailable)
            .ToList();
    }
}