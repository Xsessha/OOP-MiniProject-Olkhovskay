using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Filters;

public interface ICarFilter
{
    bool Matches(Car car);
}