using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Services;

public class RentOperationResult
{
    public Rental Rental { get; }
    public decimal BasePrice { get; }
    public decimal DiscountedPrice { get; }
    public string CustomerType { get; }

    public RentOperationResult(Rental rental, decimal basePrice, decimal discountedPrice, string customerType)
    {
        Rental = rental;
        BasePrice = basePrice;
        DiscountedPrice = discountedPrice;
        CustomerType = customerType;
    }
}

public class ReturnOperationResult
{
    public Rental Rental { get; }
    public decimal Penalty { get; }
    public decimal TotalCost { get; }
    public bool IsLate => Penalty > 0;

    public ReturnOperationResult(Rental rental, decimal penalty)
    {
        Rental = rental;
        Penalty = penalty;
        TotalCost = rental.TotalPrice + penalty;
    }
}
