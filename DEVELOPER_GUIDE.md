# Developer Guide

## Стек

- .NET 9.0
- C#
- xUnit
- coverlet.msbuild
- ReportGenerator
- System.Text.Json

## Архітектура

Проєкт поділено на чотири шари:

- `MyProject.Domain` - бізнес-сутності `Car`, `Customer`, `Rental`, value object `Money`, доменні винятки та repository interfaces.
- `MyProject.Application` - use cases: `RentalService`, `RentalFacade`, `RentalAnalyticsService`, `CustomerFactory`, event bus.
- `MyProject.Infrastructure` - in-memory repositories, JSON persistence, serialization helpers.
- `MyProject.Console` - консольний сценарій взаємодії з користувачем.

Залежності спрямовані всередину: application залежить від domain contracts, infrastructure реалізує ці contracts, console складає об'єкти разом.

## Ключові патерни

- Factory: `CustomerFactory` створює `EconomyCustomer` або `PremiumCustomer`.
- Facade: `RentalFacade` дає простий API для console UI.
- Repository: `ICarRepository`, `IRentalRepository` приховують джерело даних.
- Observer: `ApplicationEventBus`, `EventManager`, `IEventListener`, `ConsoleLogger` логують події та помилки persistence.
- Result: `Result` / `Result<T>` повертають керовані I/O помилки без аварійного завершення.

## Бізнес-правила

- Автомобіль не можна орендувати двічі одночасно.
- `economy` оренда обмежена 10 днями.
- `premium` клієнт отримує більшу знижку.
- Повернення без активної оренди завершується `RentalNotFoundException`.
- Невідомий автомобіль завершується `CarNotFoundException`.

## Розширення

Щоб додати новий тип клієнта:

1. Створити клас-нащадок `Customer`.
2. Реалізувати `CustomerType` і `GetDiscount()`.
3. Додати створення в `CustomerFactory`.
4. Додати ліміт або правило в `RentalService`, якщо потрібно.
5. Додати unit tests для ціни, factory та business flow.

Щоб додати нове джерело даних:

1. Реалізувати `ICarRepository` або `IRentalRepository`.
2. Не змінювати доменні сутності.
3. Підключити реалізацію в composition root `Program.cs`.
4. Додати integration tests для persistence сценаріїв.

## Дані та продуктивність

- `List<Car>` використовується як проста in-memory колекція для малого автопарку.
- LINQ застосовується для фільтрації, агрегації та сортування.
- `Dictionary<string, int>` використовується для підрахунку популярності моделей.
- `HashSet<string>` використовується для унікальних клієнтів.

Деталі: [docs/performance-analysis.md](docs/performance-analysis.md).

## Тести

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release
```

Coverage:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults\coverage\ /p:Threshold=85 /p:ThresholdType=line /p:ThresholdStat=total
reportgenerator -reports:tests/MyProject.Tests/TestResults/coverage/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html
```

## Правила змін

- Домен не повинен залежати від console або infrastructure.
- Бізнес-правила тримати у domain/application, а не в UI.
- Для очікуваних бізнес-помилок використовувати доменні exceptions.
- Для I/O помилок persistence повертати `Result.Fail` або логувати через event bus.
- Кожна зміна бізнес-логіки має мати тест.
