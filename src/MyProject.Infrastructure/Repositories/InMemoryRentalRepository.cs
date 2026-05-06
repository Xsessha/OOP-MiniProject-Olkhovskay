using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Infrastructure.Repositories;

public class InMemoryRentalRepository : IRentalRepository
{
    private readonly List<Rental> _rentals = new();

    public void Add(Rental rental)
    {
        _rentals.Add(rental);
    }

    public List<Rental> GetAll()
    {
        return _rentals;
    }
}