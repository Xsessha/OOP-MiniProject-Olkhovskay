using CarRentSystem.Application.Filters;
using CarRentSystem.Domain.Entities;
using Xunit;

namespace CarRentSystem.Tests.Filters;

/// <summary>
/// Unit tests for <see cref="CarFilterPipeline"/> and <see cref="CarFilters"/>.
/// Uses Theory + InlineData to verify all filter combinations without duplication.
/// </summary>
public sealed class CarFilterPipelineTests
{
    // ─── Empty pipeline ───────────────────────────────────────────────────────

    [Fact]
    public void EmptyPipeline_ShouldMatchAllCars_WhenCarIsAvailable()
    {
        var car = MakeCar(isAvailable: true, pricePerDay: 999m);
        var pipeline = new CarFilterPipeline();

        Assert.True(pipeline.Matches(car));
    }

    [Fact]
    public void EmptyPipeline_ShouldMatchAllCars_WhenCarIsNotAvailable()
    {
        // Even a rented car passes an empty pipeline — no conditions means no rejection.
        var car = MakeCar(isAvailable: false, pricePerDay: 999m);
        var pipeline = new CarFilterPipeline();

        Assert.True(pipeline.Matches(car));
    }

    // ─── Available filter ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(true,  true)]   // available → passes
    [InlineData(false, false)]  // rented   → fails
    public void AvailableFilter_ShouldMatchOnlyAvailableCars(bool isAvailable, bool expected)
    {
        var car = MakeCar(isAvailable, pricePerDay: 80m);
        var pipeline = new CarFilterPipeline().Add(CarFilters.Available());

        Assert.Equal(expected, pipeline.Matches(car));
    }

    // ─── MaxPrice filter ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(100, 80,  true)]   // maxPrice 100, car 80 → passes
    [InlineData(100, 100, true)]   // maxPrice 100, car 100 → passes (exact limit)
    [InlineData(100, 101, false)]  // maxPrice 100, car 101 → fails
    [InlineData(0,   50,  false)]  // maxPrice 0 → nothing passes
    public void MaxPriceFilter_ShouldRespectPriceLimit(
        double maxPrice, double carPrice, bool expected)
    {
        var car = MakeCar(isAvailable: true, pricePerDay: (decimal)carPrice);
        var pipeline = new CarFilterPipeline().Add(CarFilters.MaxPrice((decimal)maxPrice));

        Assert.Equal(expected, pipeline.Matches(car));
    }

    // ─── Combined AND pipeline ────────────────────────────────────────────────

    [Theory]
    [InlineData(true,  80,  100, true)]   // available, cheap enough → passes
    [InlineData(true,  101, 100, false)]  // available, too expensive → fails
    [InlineData(false, 80,  100, false)]  // not available, cheap → fails
    [InlineData(false, 101, 100, false)]  // not available, too expensive → fails
    public void Pipeline_AvailableAndMaxPrice_ShouldApplyAndLogic(
        bool isAvailable, double carPrice, double maxPrice, bool expected)
    {
        var car = MakeCar(isAvailable, pricePerDay: (decimal)carPrice);
        var pipeline = new CarFilterPipeline()
            .Add(CarFilters.Available())
            .Add(CarFilters.MaxPrice((decimal)maxPrice));

        Assert.Equal(expected, pipeline.Matches(car));
    }

    // ─── ByModel filter ───────────────────────────────────────────────────────

    [Theory]
    [InlineData("Corolla", "Corolla",   true)]   // exact match
    [InlineData("corolla", "Corolla",   true)]   // case-insensitive
    [InlineData("COROLLA", "Corolla",   true)]   // uppercase query
    [InlineData("olla",    "Corolla",   true)]   // substring match
    [InlineData("Civic",   "Corolla",   false)]  // no match
    public void ByModelFilter_ShouldBeCaseInsensitiveAndSubstring(
        string searchTerm, string carModel, bool expected)
    {
        var car = MakeCar(isAvailable: true, pricePerDay: 80m, model: carModel);
        var pipeline = new CarFilterPipeline().Add(CarFilters.ByModel(searchTerm));

        Assert.Equal(expected, pipeline.Matches(car));
    }

    // ─── ByBrand filter ───────────────────────────────────────────────────────

    [Theory]
    [InlineData("Toyota", "Toyota", true)]
    [InlineData("toyota", "Toyota", true)]   // case-insensitive
    [InlineData("Honda",  "Toyota", false)]
    public void ByBrandFilter_ShouldBeCaseInsensitiveExactMatch(
        string searchBrand, string carBrand, bool expected)
    {
        var car = MakeCar(isAvailable: true, pricePerDay: 80m, brand: carBrand);
        var pipeline = new CarFilterPipeline().Add(CarFilters.ByBrand(searchBrand));

        Assert.Equal(expected, pipeline.Matches(car));
    }

    // ─── Fluent chaining ─────────────────────────────────────────────────────

    [Fact]
    public void Add_ShouldReturnSamePipelineInstance_ForFluentChaining()
    {
        var pipeline = new CarFilterPipeline();
        var result = pipeline.Add(CarFilters.Available());

        Assert.Same(pipeline, result);
    }

    [Fact]
    public void Add_WhenNullPredicate_ShouldThrowArgumentNullException()
    {
        var pipeline = new CarFilterPipeline();
        Assert.Throws<ArgumentNullException>(() => pipeline.Add(null!));
    }

    // ─── Three-predicate pipeline ─────────────────────────────────────────────

    [Fact]
    public void Pipeline_WithThreePredicates_ShouldRequireAllToPass()
    {
        // Available + MaxPrice(100) + ByBrand("Toyota")
        var car = MakeCar(isAvailable: true, pricePerDay: 80m,
                          model: "Corolla", brand: "Toyota");

        var pipeline = new CarFilterPipeline()
            .Add(CarFilters.Available())
            .Add(CarFilters.MaxPrice(100m))
            .Add(CarFilters.ByBrand("Toyota"));

        Assert.True(pipeline.Matches(car));

        // Change brand → should fail
        var wrongBrand = MakeCar(isAvailable: true, pricePerDay: 80m,
                                 model: "Accord", brand: "Honda");
        Assert.False(pipeline.Matches(wrongBrand));
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="Car"/> test double.
    /// Adjust constructor call if your Car class has different parameters.
    /// </summary>
    private static Car MakeCar(
        bool isAvailable,
        decimal pricePerDay,
        string model = "TestModel",
        string brand = "TestBrand")
    {
        // Car(Guid id, string brand, string model, decimal pricePerDay)
        var car = new Car(Guid.NewGuid(), brand, model, pricePerDay);

        // If car starts as available by default, rent it to make it unavailable.
        if (!isAvailable)
            car.Rent();

        return car;
    }
}