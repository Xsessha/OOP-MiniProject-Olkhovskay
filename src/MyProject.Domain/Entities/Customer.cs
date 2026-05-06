namespace MyProject.Domain.Entities;

public abstract class Customer
{
    public string Name { get; private set; }

    protected Customer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        Name = name;
    }

    public abstract decimal GetDiscount();
}