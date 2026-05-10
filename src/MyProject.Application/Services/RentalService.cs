using System.Linq;
using MyProject.Application.Factories;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Services;

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

    public RentOperationResult RentCar(string customerName, string customerType, Guid carId, int days)
    {
        ValidateCustomerLimit(customerType, days);

        var car = _carReadRepository.GetById(carId);

        if (car == null)
            throw new CarNotFoundException(carId);

        car.Rent();
        _carWriteRepository.Update(car);

        var customer = CustomerFactory.Create(customerName, customerType);

        var basePrice = car.PricePerDay * days;
        var discountedPrice = ApplyDiscount(basePrice, customerType);

        var rental = new Rental(car, customer, days);

        _rentalWriteRepository.Add(rental);

        return new RentOperationResult(rental, basePrice, discountedPrice, customerType);
    }

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

        if (_dateTimeProvider.Now <= expected)
            return 0;

        var lateDays = (_dateTimeProvider.Now - expected).Days;

        return lateDays * rental.Car.PricePerDay * 1.5m;
    }

    public List<Car> GetAvailableCars()
    {
        return _carReadRepository.GetAll()
            .Where(c => c.IsAvailable)
            .ToList();
    }
}