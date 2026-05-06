namespace MyProject.Application.Interfaces;

public interface IRentalService
{
    void RentCar(string customerName, Guid carId);
}