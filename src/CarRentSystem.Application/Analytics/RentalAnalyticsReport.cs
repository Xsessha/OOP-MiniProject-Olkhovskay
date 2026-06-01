namespace CarRentSystem.Application.Analytics;

public sealed record RentalModelStatistic(
    string Model,
    int RentalCount,
    decimal Revenue);

public sealed record RentalAnalyticsReport(
    int RentalCount,
    int ActiveRentalCount,
    decimal TotalRevenue,
    double AverageRentalDays,
    IReadOnlyList<RentalModelStatistic> ModelStatistics)
{
    public string? MostPopularModel => ModelStatistics.FirstOrDefault()?.Model;
}
