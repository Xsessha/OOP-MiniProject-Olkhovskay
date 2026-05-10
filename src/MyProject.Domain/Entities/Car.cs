using System.Text.Json.Serialization;
using MyProject.Domain.Configuration;
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
        => CarPricingConfiguration.GetPriceForModel(model);

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