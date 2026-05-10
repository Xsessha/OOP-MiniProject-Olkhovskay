namespace MyProject.Domain.Entities;

public class Rental
{
    public Car Car { get; private set; }
    public Customer Customer { get; private set; }
    public int Days { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime RentedAt { get; private set; }
    public decimal LatePenalty { get; private set; }

    public Rental(Car car, Customer customer, int days)
    {
        if (car == null)
            throw new ArgumentNullException(nameof(car));
        if (customer == null)
            throw new ArgumentNullException(nameof(customer));
        if (days <= 0)
            throw new ArgumentException("Days must be at least 1");

        if (days > 365)
            throw new ArgumentException("Days cannot exceed 365");

        Car = car;
        Customer = customer;
        Days = days;
        RentedAt = DateTime.Now;
        LatePenalty = 0;

        var discount = customer.GetDiscount();
        TotalPrice = car.PricePerDay * days * (1 - discount);
    }

    public void CalculatePenalty()
    {
        var expectedReturnDate = RentedAt.AddDays(Days);
        var actualReturnDate = DateTime.Now;

        if (actualReturnDate > expectedReturnDate)
        {
            var daysLate = (int)Math.Ceiling((actualReturnDate - expectedReturnDate).TotalDays);
            LatePenalty = Car.PricePerDay * daysLate * 1.5m;
        }
    }

    public decimal GetTotalCost()
    {
        return TotalPrice + LatePenalty;
    }
}