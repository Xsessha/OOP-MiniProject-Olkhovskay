using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Interfaces;

namespace CarRentSystem.Infrastructure.Repositories;

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