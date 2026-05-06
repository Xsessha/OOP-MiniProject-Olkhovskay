namespace MyProject.Domain.Strategies;

public interface IPricingStrategy
{
    decimal CalculatePrice(decimal basePrice);
}