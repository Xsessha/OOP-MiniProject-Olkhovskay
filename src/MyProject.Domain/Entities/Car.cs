using System.Text.Json.Serialization;
using MyProject.Domain.Exceptions;

namespace MyProject.Domain.Entities;

public class Car
{
    public Guid Id { get; private set; }
    public string Model { get; private set; }
    public bool IsAvailable { get; private set; }

    public decimal PricePerDay { get; private set; }

    [JsonConstructor]
    public Car(Guid id, string model, bool isAvailable, decimal pricePerDay)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty");

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Model = model;
        IsAvailable = isAvailable;
        PricePerDay = NormalizePrice(model, pricePerDay);
    }

    public Car(string model)
        : this(Guid.NewGuid(), model, true, 0)
    {
    }

    public Car(string model, decimal pricePerDay)
        : this(Guid.NewGuid(), model, true, pricePerDay)
    {
    }

    private static decimal NormalizePrice(string model, decimal pricePerDay)
    {
        var defaultPrice = GetDefaultPrice(model);

        if (pricePerDay <= 0)
            return defaultPrice;

        if (pricePerDay == 60 && defaultPrice != 60)
            return defaultPrice;

        return pricePerDay;
    }

    private static decimal GetDefaultPrice(string model)
        => model switch
        {
            "BMW X5" => 120,
            "Audi A6" => 110,
            "Toyota Camry" => 70,
            "Mercedes-Benz S-Class" => 200,
            "Tesla Model 3" => 150,
            "Honda Civic" => 65,
            "Ford Mustang" => 130,
            "Volkswagen Golf" => 75,
            "Porsche 911" => 250,
            "Nissan Rogue" => 80,
            "Hyundai Tucson" => 72,
            "Kia Sportage" => 68,
            "Volvo XC90" => 90,
            "Mazda CX-5" => 78,
            "Subaru Outback" => 82,
            "Lexus RX 350" => 140,
            "Chevrolet Camaro" => 125,
            "Jaguar F-Type" => 220,
            _ => 60
        };

    public void Rent()
    {
        if (!IsAvailable)
            throw new CarAlreadyRentedException(Id);

        IsAvailable = false;
    }

    public void Return()
    {
        IsAvailable = true;
    }
}