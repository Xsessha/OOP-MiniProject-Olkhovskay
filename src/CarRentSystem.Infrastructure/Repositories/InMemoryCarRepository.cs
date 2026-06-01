using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Interfaces;

namespace CarRentSystem.Infrastructure.Repositories;

public class InMemoryCarRepository : ICarRepository
{
    private List<Car> _cars = new();

    public InMemoryCarRepository()
    {
    }

    public InMemoryCarRepository(IEnumerable<Car> cars)
    {
        _cars = cars?.ToList() ?? new List<Car>();
    }

    public void Add(Car car)
    {
        _cars.Add(car);
    }

    public Car? GetById(Guid id)
    {
        return _cars.FirstOrDefault(c => c.Id == id);
    }

    public List<Car> GetAll()
    {
        return _cars;
    }

    public void Update(Car car)
    {
        var index = _cars.FindIndex(c => c.Id == car.Id);

        if (index >= 0)
        {
            _cars[index] = car;
        }
    }
}