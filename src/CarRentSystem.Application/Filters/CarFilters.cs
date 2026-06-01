using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Filters;


public static class CarFilters
{
    public static Func<Car, bool> Available() =>
        car => car.IsAvailable;

    public static Func<Car, bool> ByModel(string model) =>
        car => car.Model.Contains(model, StringComparison.OrdinalIgnoreCase);


    public static Func<Car, bool> ByBrand(string brand) =>
        car => car.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase);

    public static Func<Car, bool> MaxPrice(decimal maxPricePerDay) =>
        car => car.PricePerDay <= maxPricePerDay;

    public static Func<Car, bool> MinPrice(decimal minPricePerDay) =>
        car => car.PricePerDay >= minPricePerDay;
}