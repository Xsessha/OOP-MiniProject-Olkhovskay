using MyProject.Domain.Entities;

namespace MyProject.Domain.Interfaces;

public interface IRentalRepository
{
    void Add(Rental rental);
    List<Rental> GetAll();
}