namespace MyProject.Domain.Strategies;

public class DiscountPricingStrategy : IPricingStrategy
{
    private readonly decimal _discount;

    public DiscountPricingStrategy(decimal discount)
    {
        _discount = discount;
    }

    public decimal CalculatePrice(decimal basePrice)
    {
        return basePrice * (1 - _discount);
    }
}