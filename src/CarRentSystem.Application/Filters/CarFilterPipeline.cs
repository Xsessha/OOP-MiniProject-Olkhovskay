using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Filters;

public sealed class CarFilterPipeline : ICarFilter
{
    private readonly List<Func<Car, bool>> _predicates = new();

    public CarFilterPipeline Add(Func<Car, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _predicates.Add(predicate);
        return this;
    }

    
    public bool Matches(Car car) =>
        _predicates.All(p => p(car));
}