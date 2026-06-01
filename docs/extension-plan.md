# Extension Plan — Три залежні розширення

> Самостійна робота №29 · CarRentSystem · Варіант 1: Делегати + LINQ + тести

---

## Загальна ідея ланцюжка

```
Розширення А                Розширення Б                 Розширення В
─────────────               ─────────────                ─────────────
ICarFilter + pipeline  ──►  SearchCars LINQ + analytics  ──►  Theory-тести pipeline
(нова точка розширення)     (нова поведінка, що            (підтвердження Б,
                             використовує А)               branch coverage)
```

---

## Розширення А — `ICarFilter` з делегат-pipeline

### Мотивація

Зараз пошук авто в `RentalFacade` та `RentalAnalyticsService` реалізований через жорстко закодовані лямбди. Якщо додати нову умову (наприклад, фільтр за маркою або роком), треба змінювати facade — порушення OCP.

### Що реалізувати

**`src/MyProject.Application/Filters/ICarFilter.cs`**
```csharp
namespace MyProject.Application.Filters;

/// <summary>Composable predicate for car search pipeline.</summary>
public interface ICarFilter
{
    bool Matches(Car car);
}
```

**`src/MyProject.Application/Filters/CarFilterPipeline.cs`**
```csharp
namespace MyProject.Application.Filters;

/// <summary>
/// Combines multiple ICarFilter instances via AND-logic.
/// Uses Func&lt;Car, bool&gt; delegate internally.
/// </summary>
public sealed class CarFilterPipeline : ICarFilter
{
    private readonly List<Func<Car, bool>> _predicates = new();

    public CarFilterPipeline Add(Func<Car, bool> predicate)
    {
        _predicates.Add(predicate);
        return this; // fluent API
    }

    public bool Matches(Car car) =>
        _predicates.All(p => p(car));
}
```

**`src/MyProject.Application/Filters/CarFilters.cs`** — готові фільтри:
```csharp
public static class CarFilters
{
    public static Func<Car, bool> Available()        => c => c.IsAvailable;
    public static Func<Car, bool> ByModel(string m)  => c => c.Model.Contains(m, StringComparison.OrdinalIgnoreCase);
    public static Func<Car, bool> MaxPrice(decimal p) => c => c.PricePerDay <= p;
    public static Func<Car, bool> ByBrand(string b)  => c => c.Brand.Equals(b, StringComparison.OrdinalIgnoreCase);
}
```

### Вхідний артефакт

Поточний `ICarRepository` і доменна модель `Car`.

### Результат

Нова точка розширення: будь-який новий фільтр додається без зміни сервісів — `Add(CarFilters.MaxPrice(100))`.

---

## Розширення Б — `SearchCars` в `RentalFacade` + розширена аналітика

### Мотивація

Розширення А дало composable-фільтр. Тепер його треба підключити до реального сценарію: пошук авто в фасаді та аналітика з LINQ join.

### Що реалізувати

**Метод у `RentalFacade`:**
```csharp
/// <summary>Returns cars matching all conditions in the pipeline.</summary>
public IEnumerable<Car> SearchCars(CarFilterPipeline pipeline) =>
    _carRepository.GetAll().Where(pipeline.Matches);
```

**Новий метод в `RentalAnalyticsService`:**
```csharp
/// <summary>
/// JOIN-проєкція: для кожної оренди повертає пару (rental, car).
/// Дозволяє будувати звіти з даними і про оренду, і про авто.
/// </summary>
public IEnumerable<RentalCarProjection> GetRentalWithCar() =>
    from rental in _rentalRepository.GetAll()
    join car in _carRepository.GetAll()
        on rental.CarId equals car.Id
    select new RentalCarProjection(rental, car);
```

**`RentalCarProjection`** — record для результату join:
```csharp
public sealed record RentalCarProjection(Rental Rental, Car Car);
```

**Оновлення консольного меню** — нова опція "Пошук авто за фільтром":
```csharp
case "5":
    var pipeline = new CarFilterPipeline()
        .Add(CarFilters.Available())
        .Add(CarFilters.MaxPrice(maxPrice));
    var results = _facade.SearchCars(pipeline);
    // вивести результати
    break;
```

### Вхідний артефакт

`CarFilterPipeline` з Розширення А, `ICarRepository`, `IRentalRepository`.

### Результат

Реальний сценарій пошуку без зміни facade-інтерфейсу. LINQ join між `Rental` і `Car`.

---

## Розширення В — параметризовані Theory-тести

### Мотивація

Pipeline і join треба верифікувати для граничних кейсів: порожній pipeline, комбінація фільтрів, join з відсутніми avто.

### Що реалізувати

**`tests/MyProject.Tests/Filters/CarFilterPipelineTests.cs`:**
```csharp
public class CarFilterPipelineTests
{
    public static IEnumerable<object[]> FilterCases() => new[]
    {
        new object[] { /* available + maxPrice */ 80m,  true,  true  },
        new object[] { /* maxPrice too low    */ 10m,  true,  false },
        new object[] { /* car not available   */ 80m,  false, false },
    };

    [Theory, MemberData(nameof(FilterCases))]
    public void Pipeline_ShouldMatch_AsExpected(decimal maxPrice, bool isAvailable, bool expected)
    {
        var car = CreateCar(isAvailable, pricePerDay: 50m);
        var pipeline = new CarFilterPipeline()
            .Add(CarFilters.Available())
            .Add(CarFilters.MaxPrice(maxPrice));

        Assert.Equal(expected, pipeline.Matches(car));
    }
}
```

**`tests/MyProject.Tests/Analytics/RentalCarProjectionTests.cs`:**
```csharp
[Fact]
public void GetRentalWithCar_ShouldReturnCorrectProjection()
{
    // arrange: mock repos
    // act: analytics.GetRentalWithCar()
    // assert: projection має правильний Car.Model і Rental.TotalPrice
}

[Fact]
public void GetRentalWithCar_WhenCarMissing_ShouldReturnEmpty()
{
    // join з відсутнім CarId → порожній результат
}
```

### Вхідний артефакт

`CarFilterPipeline` (А) і `GetRentalWithCar` (Б).

### Результат

Branch coverage для pipeline піднімається. Тести фіксують поведінку для всіх граничних кейсів.

---

## Залежність між кроками

| Крок | Залежить від |
|---|---|
| А — `ICarFilter` + `CarFilterPipeline` | Поточна доменна модель `Car` |
| Б — `SearchCars` + join-аналітика | `CarFilterPipeline` з А |
| В — Theory-тести | `CarFilterPipeline` (А) і `GetRentalWithCar` (Б) |