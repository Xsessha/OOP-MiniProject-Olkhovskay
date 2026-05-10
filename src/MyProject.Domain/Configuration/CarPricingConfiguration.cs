namespace MyProject.Domain.Configuration;

/// <summary>
/// Centralized configuration for car model pricing.
/// Reduces cyclomatic complexity of Car.GetDefaultPrice().
/// </summary>
public static class CarPricingConfiguration
{
    private static readonly Dictionary<string, decimal> ModelPrices = new()
    {
        { "BMW X5", 120 },
        { "Audi A6", 110 },
        { "Toyota Camry", 70 },
        { "Mercedes-Benz S-Class", 200 },
        { "Tesla Model 3", 150 },
        { "Honda Civic", 65 },
        { "Ford Mustang", 130 },
        { "Volkswagen Golf", 75 },
        { "Porsche 911", 250 },
        { "Nissan Rogue", 80 },
        { "Hyundai Tucson", 72 },
        { "Kia Sportage", 68 },
        { "Volvo XC90", 90 },
        { "Mazda CX-5", 78 },
        { "Subaru Outback", 82 },
        { "Lexus RX 350", 140 },
        { "Chevrolet Camaro", 125 },
        { "Jaguar F-Type", 220 }
    };

    private const decimal DefaultPrice = 60;

    /// <summary>
    /// Gets the default price for a given car model.
    /// Returns DefaultPrice (60) if model is not found.
    /// </summary>
    public static decimal GetPriceForModel(string model)
    {
        return string.IsNullOrWhiteSpace(model) 
            ? DefaultPrice 
            : ModelPrices.TryGetValue(model, out var price) 
                ? price 
                : DefaultPrice;
    }

    /// <summary>
    /// Gets all configured car models and their prices.
    /// </summary>
    public static IReadOnlyDictionary<string, decimal> GetAllModelPrices()
    {
        return ModelPrices.AsReadOnly();
    }
}
