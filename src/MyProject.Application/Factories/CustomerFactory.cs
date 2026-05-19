using MyProject.Domain.Entities;

namespace MyProject.Application.Factories;

/// <summary>
/// Creates concrete customer types from user-facing customer type input.
/// </summary>
public static class CustomerFactory
{
    /// <summary>
    /// Creates a customer instance for supported types: economy or premium.
    /// </summary>
    public static Customer Create(string name, string type)
    {
        var normalizedType = string.IsNullOrWhiteSpace(type)
            ? string.Empty
            : type.Trim().ToLowerInvariant();

        return normalizedType switch
        {
            "premium" => new PremiumCustomer(name),
            "economy" => new EconomyCustomer(name),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }
}
