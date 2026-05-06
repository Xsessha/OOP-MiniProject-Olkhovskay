namespace MyProject.Domain.Entities;

public class Car
{
    public Guid Id { get; private set; }
    public string Model { get; private set; }
    public bool IsAvailable { get; private set; }

    public Car(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty");

        Id = Guid.NewGuid();
        Model = model;
        IsAvailable = true;
    }

    public void Rent()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Car already rented");

        IsAvailable = false;
    }

    public void Return()
    {
        IsAvailable = true;
    }
}