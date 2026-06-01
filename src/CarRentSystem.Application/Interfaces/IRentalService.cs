namespace CarRentSystem.Application.Interfaces;

public interface IRentalService
{
    void RentCar(string customerName, Guid carId);
}