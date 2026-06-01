using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Domain.Interfaces;

public interface ICarReadRepository
{
    Car? GetById(Guid id);
    List<Car> GetAll();
}

public interface ICarWriteRepository
{
    void Add(Car car);
    void Update(Car car);
}

public interface ICarRepository : ICarReadRepository, ICarWriteRepository
{
}
