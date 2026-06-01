using System.Linq;
using CarRentSystem.Application.Factories;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Exceptions;
using CarRentSystem.Domain.Interfaces;

namespace CarRentSystem.Application.Services;

/// <summary>
/// Coordinates the main rental use cases and keeps domain rules outside the console UI.
/// </summary>
public class RentalService
{
    private readonly ICarReadRepository _carReadRepository;
    private readonly ICarWriteRepository _carWriteRepository;
    private readonly IRentalReadRepository _rentalReadRepository;
    private readonly IRentalWriteRepository _rentalWriteRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RentalService(
        ICarReadRepository carReadRepository,
        ICarWriteRepository carWriteRepository,
        IRentalReadRepository rentalReadRepository,
        IRentalWriteRepository rentalWriteRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _carReadRepository = carReadRepository;
        _carWriteRepository = carWriteRepository;
        _rentalReadRepository = rentalReadRepository;
        _rentalWriteRepository = rentalWriteRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public RentalService(ICarRepository carRepository, IRentalRepository rentalRepository)
        : this(carRepository, carRepository, rentalRepository, rentalRepository, new SystemDateTimeProvider())
    {
    }

    public RentalService(ICarRepository carRepository, IRentalRepository rentalRepository, IDateTimeProvider dateTimeProvider)
        : this(carRepository, carRepository, rentalRepository, rentalRepository, dateTimeProvider)
    {
    }

    /// <summary>
    /// Rents an available car to a customer and returns the calculated pricing details.
    /// </summary>
    public RentOperationResult RentCar(string customerName, string customerType, Guid carId, int days)
    {
        var normalizedCustomerType = NormalizeCustomerType(customerType);
        ValidateRentalDays(days);
        ValidateCustomerLimit(normalizedCustomerType, days);

        var car = _carReadRepository.GetById(carId);

        if (car == null)
            throw new CarNotFoundException(carId);

        car.Rent();
        _carWriteRepository.Update(car);

        var customer = CustomerFactory.Create(customerName, normalizedCustomerType);

        var basePrice = car.PricePerDay * days;
        var discountedPrice = ApplyCustomerDiscount(basePrice, customer);

        var rental = new Rental(car, customer, days);

        _rentalWriteRepository.Add(rental);

        return new RentOperationResult(rental, basePrice, discountedPrice, normalizedCustomerType);
    }

    /// <summary>
    /// Returns a rented car and calculates a late-return penalty when needed.
    /// </summary>
    public ReturnOperationResult ReturnCar(Guid carId)
    {
        var car = _carReadRepository.GetById(carId);

        if (car == null)
            throw new CarNotFoundException(carId);

        var rental = _rentalReadRepository.GetAll()
            .FirstOrDefault(r => r.Car.Id == carId);

        if (rental == null)
            throw new RentalNotFoundException(carId);

        car.Return();
        _carWriteRepository.Update(car);

        var penalty = CalculatePenalty(rental);

        return new ReturnOperationResult(rental, penalty);
    }

    private static decimal ApplyCustomerDiscount(decimal price, Customer customer)
    {
        return price * (1 - customer.GetDiscount());
    }

    private static string NormalizeCustomerType(string type)
    {
        return string.IsNullOrWhiteSpace(type)
            ? string.Empty
            : type.Trim().ToLowerInvariant();
    }

    private static void ValidateRentalDays(int days)
    {
        if (days <= 0)
            throw new ArgumentException("Days must be at least 1", nameof(days));

        if (days > 365)
            throw new ArgumentException("Days cannot exceed 365", nameof(days));
    }

    private static void ValidateCustomerLimit(string type, int days)
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

        if (_dateTimeProvider.Now <= expected)
            return 0;

        var lateDays = (_dateTimeProvider.Now - expected).Days;

        return lateDays * rental.Car.PricePerDay * 1.5m;
    }

    /// <summary>
    /// Returns cars that can currently be rented.
    /// </summary>
    public List<Car> GetAvailableCars()
    {
        return _carReadRepository.GetAll()
            .Where(c => c.IsAvailable)
            .ToList();
    }
}
