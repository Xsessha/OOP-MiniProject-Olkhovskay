using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Analytics;


public sealed record RentalCarProjection(Rental Rental, Car Car);