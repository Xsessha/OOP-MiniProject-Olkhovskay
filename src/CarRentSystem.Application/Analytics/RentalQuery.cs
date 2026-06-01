using CarRentSystem.Domain.Entities;

namespace CarRentSystem.Application.Analytics;


public sealed class RentalQuery
{
    private readonly IReadOnlyList<Func<Rental, bool>> _predicates;
    private readonly IReadOnlyList<string> _cacheParts;

    private RentalQuery(
        IReadOnlyList<Func<Rental, bool>> predicates,
        IReadOnlyList<string> cacheParts)
    {
        _predicates = predicates;
        _cacheParts = cacheParts;
    }

    public static RentalQuery All { get; } = new(
        Array.Empty<Func<Rental, bool>>(),
        new[] { "all" });

    public string CacheKey => string.Join("|", _cacheParts);

    public RentalQuery Where(string cachePart, Func<Rental, bool> predicate)
    {
        if (string.IsNullOrWhiteSpace(cachePart))
            throw new ArgumentException("Cache key part cannot be empty.", nameof(cachePart));

        ArgumentNullException.ThrowIfNull(predicate);

        return new RentalQuery(
            _predicates.Append(predicate).ToArray(),
            _cacheParts.Append(NormalizeCachePart(cachePart)).ToArray());
    }

    public RentalQuery ActiveOnly()
    {
        return Where("active", rental => !rental.Car.IsAvailable);
    }

    public RentalQuery CustomerContains(string value)
    {
        var normalized = NormalizeSearchValue(value, nameof(value));

        return Where(
            $"customer:{normalized}",
            rental => rental.Customer.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase));
    }

    public RentalQuery ModelContains(string value)
    {
        var normalized = NormalizeSearchValue(value, nameof(value));

        return Where(
            $"model:{normalized}",
            rental => rental.Car.Model.Contains(normalized, StringComparison.OrdinalIgnoreCase));
    }

    public RentalQuery MinimumDays(int days)
    {
        if (days <= 0)
            throw new ArgumentException("Minimum days must be at least 1.", nameof(days));

        return Where($"min-days:{days}", rental => rental.Days >= days);
    }

    public bool Matches(Rental rental)
    {
        ArgumentNullException.ThrowIfNull(rental);

        return _predicates.All(predicate => predicate(rental));
    }

    public IEnumerable<Rental> Apply(IEnumerable<Rental> rentals)
    {
        ArgumentNullException.ThrowIfNull(rentals);

        return rentals.Where(Matches);
    }

    private static string NormalizeSearchValue(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Search value cannot be empty.", parameterName);

        return value.Trim();
    }

    private static string NormalizeCachePart(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
