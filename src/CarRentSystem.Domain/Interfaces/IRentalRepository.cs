using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Domain.Interfaces;

public interface IRentalReadRepository
{
    List<Rental> GetAll();
}

public interface IRentalWriteRepository
{
    void Add(Rental rental);
}

public interface IRentalRepository : IRentalReadRepository, IRentalWriteRepository
{
}
