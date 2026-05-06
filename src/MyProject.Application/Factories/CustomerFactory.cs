using MyProject.Domain.Entities;

namespace MyProject.Application.Factories;

public static class CustomerFactory
{
    public static Customer Create(string name, string type)
    {
        return type.ToLower() switch
        {
            "premium" => new PremiumCustomer(name),
            "economy" => new EconomyCustomer(name),
            _ => throw new ArgumentException("Invalid customer type")
        };
    }
}