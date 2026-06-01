namespace CarRentSystem.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class CarNotFoundException : DomainException
{
    public Guid CarId { get; }

    public CarNotFoundException(Guid carId)
        : base($"Car with ID {carId} was not found") => CarId = carId;
}

public class CarAlreadyRentedException : DomainException
{
    public Guid CarId { get; }

    public CarAlreadyRentedException(Guid carId)
        : base($"Car with ID {carId} is already rented") => CarId = carId;
}

public class RentalNotFoundException : DomainException
{
    public Guid CarId { get; }

    public RentalNotFoundException(Guid carId)
        : base($"No active rental found for car with ID {carId}") => CarId = carId;
}

public class InvalidCustomerTypeException : DomainException
{
    public string CustomerType { get; }

    public InvalidCustomerTypeException(string customerType)
        : base($"Invalid customer type: {customerType}. Expected 'economy' or 'premium'") => CustomerType = customerType;
}

public class RentalLimitExceededException : DomainException
{
    public string CustomerType { get; }
    public int MaxDays { get; }

    public RentalLimitExceededException(string customerType, int maxDays)
        : base($"{customerType} customers cannot rent for more than {maxDays} days") 
    {
        CustomerType = customerType;
        MaxDays = maxDays;
    }
}