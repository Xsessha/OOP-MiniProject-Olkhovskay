using MyProject.Domain.Entities;

namespace MyProject.Domain.Interfaces;

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
