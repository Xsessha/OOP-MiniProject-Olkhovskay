namespace MyProject.Domain.Entities;

public class Rental
{
    public Guid Id { get; }
    public Car Car { get; }
    public Customer Customer { get; }
    public DateTime Date { get; }

    public Rental(Car car, Customer customer)
    {
        Id = Guid.NewGuid();
        Car = car ?? throw new ArgumentNullException(nameof(car));
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Date = DateTime.Now;
    }
}