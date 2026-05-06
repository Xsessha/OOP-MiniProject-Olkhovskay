using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Infrastructure.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private readonly Dictionary<Guid, Car> _cars = new();

    public void Add(Car car)
    {
        _cars[car.Id] = car;
    }

    public Car? GetById(Guid id)
    {
        return _cars.TryGetValue(id, out var car) ? car : null;
    }

    public List<Car> GetAll()
    {
        return _cars.Values.ToList();
    }
}