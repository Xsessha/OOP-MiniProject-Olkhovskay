namespace MyProject.Console.Handlers;

using System;
using MyProject.Application.Services;
using MyProject.Domain.Interfaces;

public class RentCarHandler
{
    private readonly RentalService _service;
    private readonly ICarRepository _carRepository;

    public RentCarHandler(RentalService service, ICarRepository carRepository)
    {
        _service = service;
        _carRepository = carRepository;
    }

    public void Handle()
    {
        System.Console.WriteLine("\n--- AVAILABLE CARS ---");
        var cars = _carRepository.GetAll();
        
        foreach (var car in cars)
        {
            string status = car.IsAvailable ? "Available" : "Rented";
            System.Console.WriteLine($"{car.Id} | {car.Model} | {status}");
        }

        System.Console.Write("\nEnter your name: ");
        var name = System.Console.ReadLine();

        System.Console.Write("Enter customer type (economy/premium): ");
        var customerType = System.Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

        System.Console.Write("Enter car ID: ");
        var idInput = System.Console.ReadLine();

        System.Console.Write("Enter number of rental days: ");
        var daysInput = System.Console.ReadLine();

        if (!Guid.TryParse(idInput, out Guid carId))
        {
            System.Console.WriteLine("⚠️ Invalid ID format. Try copying a Guid from the list above.");
            return;
        }

        if (customerType is not "economy" and not "premium")
        {
            System.Console.WriteLine("⚠️ Invalid customer type. Use 'economy' or 'premium'.");
            return;
        }

        if (!int.TryParse(daysInput, out var days) || days <= 0)
        {
            System.Console.WriteLine("Enter a valid number of days (1 or more).");
            return;
        }

        try
        {
            _service.RentCar(name!, customerType, carId, days);
            System.Console.WriteLine("Successfully rented!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }
    }
}