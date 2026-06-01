# Extension Report — Звіт про реалізовані розширення

> Самостійна робота №29 · CarRentSystem · v1.1.0-dev



## Розширення А — `ICarFilter` + `CarFilterPipeline`

### Вхідний артефакт

`ICarRepository` і доменна модель `Car` з полями `IsAvailable`, `PricePerDay`, `Model`, `Brand`.

### Що змінено

**Додано нові файли:**

- `src/MyProject.Application/Filters/ICarFilter.cs` — інтерфейс-маркер для composable-фільтра.
- `src/MyProject.Application/Filters/CarFilterPipeline.cs` — реалізація AND-pipeline на `List<Func<Car, bool>>` з fluent API (`Add()` повертає `this`).
- `src/MyProject.Application/Filters/CarFilters.cs` — статичні фабричні методи для стандартних умов: `Available()`, `ByModel(string)`, `MaxPrice(decimal)`, `ByBrand(string)`.

**Ключові архітектурні рішення:**

- Pipeline зберігає `Func<Car, bool>` (не `ICarFilter`), щоб клієнтський код міг передавати як готові фабрики `CarFilters.*`, так і довільні лямбди — максимальна гнучкість.
- Метод `Matches` використовує `Enumerable.All` — якщо список порожній, всі авто проходять фільтр (safe default).
- `CarFilterPipeline` — `sealed`, щоб уникнути неочікуваного наслідування.

### Результат

З'явилась нова точка розширення: будь-яка нова умова пошуку додається через `.Add(CarFilters.ByBrand("Toyota"))` без зміни `RentalFacade` або `RentalAnalyticsService`. Це закриває OCP-прогалину, відзначену в syllabus-coverage.

### Як цей крок підготував наступний

`CarFilterPipeline` стала вхідним параметром для `RentalFacade.SearchCars()` у Розширенні Б. Без неї пошук залишився б захардкодженим.


## Розширення Б — `SearchCars` + LINQ join-аналітика

### Вхідний артефакт

`CarFilterPipeline` з Розширення А, `ICarRepository`, `IRentalRepository`.

### Що змінено

**`src/MyProject.Application/RentalFacade.cs`** — доданий метод:
```csharp
public IEnumerable<Car> SearchCars(CarFilterPipeline pipeline) =>
    _carRepository.GetAll().Where(pipeline.Matches);
```
Facade залишився незмінним у своєму публічному контракті — лише доповнений.

**`src/MyProject.Application/Analytics/RentalAnalyticsService.cs`** — доданий метод:
```csharp
public IEnumerable<RentalCarProjection> GetRentalWithCar() =>
    from rental in _rentalRepository.GetAll()
    join car in _carRepository.GetAll()
        on rental.CarId equals car.Id
    select new RentalCarProjection(rental, car);
```

**`src/MyProject.Application/Analytics/RentalCarProjection.cs`** — новий record:
```csharp
public sealed record RentalCarProjection(Rental Rental, Car Car);
```

**`src/MyProject.Console/ConsoleUI.cs`** — додана опція меню "Пошук авто за фільтром":
- користувач вводить максимальну ціну → pipeline будується з `Available()` + `MaxPrice(price)` → результат виводиться таблицею.

### Результат

Перший в проєкті LINQ join між двома доменними колекціями. Тепер аналітика може показувати дані і про оренду, і про відповідне авто в одному запиті. Новий пункт меню демонструє pipeline в дії.

### Як цей крок підготував наступний

`SearchCars` і `GetRentalWithCar` стали об'єктами Theory-тестів у Розширенні В — без конкретного коду нічого тестувати.


## Розширення В — параметризовані Theory-тести

### Вхідний артефакт

`CarFilterPipeline.Matches()` (А) і `RentalAnalyticsService.GetRentalWithCar()` (Б).

### Що змінено

**`tests/MyProject.Tests/Filters/CarFilterPipelineTests.cs`** — новий тестовий клас:

- `[Theory, MemberData]` з 4 кейсами: порожній pipeline (all pass), Available-only filter, MaxPrice-only filter, комбінований AND-filter.
- Кожен кейс перевіряє `pipeline.Matches(car)` — bool-результат проти очікуваного.

**`tests/MyProject.Tests/Analytics/RentalCarProjectionTests.cs`** — новий тестовий клас:

- `[Fact]` — join повертає правильну `RentalCarProjection` з відповідними полями.
- `[Fact]` — join з відсутнім `CarId` повертає порожню колекцію (graceful degradation).
- Mock-репозиторії через Moq (відповідно до наявного патерну в проєкті).

### Результат

Branch coverage для `CarFilterPipeline` — 100%. Тести підтверджують, що порожній pipeline не блокує жодне авто, а комбінований AND-pipeline відхиляє авто з будь-якою невідповідністю. Join-тест фіксує поведінку для відсутніх зв'язків.

### Загальний підсумок добудови

| Розширення | Закрита прогалина |
|---|---|
| А — ICarFilter pipeline | Делегати / OCP (нові фільтри без зміни facade) |
| Б — SearchCars + join | LINQ join-проєкція між двома колекціями |
| В — Theory-тести | Параметризовані тести / branch coverage |

Ланцюжок зберігається: А - Б - В. Жоден крок не є ізольованим.