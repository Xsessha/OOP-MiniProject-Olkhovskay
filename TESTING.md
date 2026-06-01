# Testing

## Команди

Запуск усіх тестів:

```bash
dotnet test tests/CarRentSystemstem.TeCarRCarRentSystemject.Tests.csproj --configuration Release
```

Coverage з line threshold 85%:

```bash
dotnet test tests/CarRentSystemstem.Tests/CarRentSystem.Tests.csproj --configuration Release --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults\coverage\ /p:Threshold=85 /p:ThresholdType=line /p:ThresholdStat=total
```

HTML-звіт:

```bash
reportgenerator -reports:tests/CarRentSystem.Tests/TestResults/coverage/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html
```

## Поточний результат

Останній локальний прогін: 2026-05-19.

```text
Passed: 217
Failed: 0
Skipped: 0
```

Загальний результат проекту (Total)
- Тести перевіряють 91.34% від усіх рядків коду в проекті.
- Тести проходять через 87.50% усіх можливих варіантів розвитку подій (розгалужень) у коді.
- Загалом у проекті протестовано 91.97% усіх написаних методів (функцій).

1. 
- Модуль CarRentSystem.Domain (Доменна логіка)
Тести покривають 95.96% рядків коду цього модуля.
- Протестовано 96.66% усіх умов та логічних розгалужень.
- Тести викликають і перевіряють 93.87% методів.
- Це найкращий показник у всьому проекті.

2. 
- Модуль CarRentSystem.Application (Прикладний шар)
Тести покривають 90.58% рядків коду.
Логічні розгалуження та умови перевірені на 86.84%.
- Тестами охоплено 87.93% усіх методів.

3. 
- Модуль CarRentSystem.Infrastructure (Інфраструктура)
Тести покривають 86.86% рядків коду.
- Логічні розгалуження та перевірки умов виконані на 78.57%.
- Тести успішно викликають 96.66% усіх методів цього модуля.
## Що покрито

- доменні інваріанти `Car`, `Rental`, `Customer`, `Money`;
- бізнес-сценарії оренди та повернення;
- винятки `CarNotFoundException`, `CarAlreadyRentedException`, `RentalNotFoundException`, `InvalidCustomerTypeException`, `RentalLimitExceededException`;
- pricing behavior для economy/premium клієнтів;
- `RentalService`, `RentalFacade`, `RentalAnalyticsService`;
- repositories та persistence;
- JSON load/save, corrupted JSON, missing file, invalid path;
- event bus та listeners;
- `Result` / `Result<T>`.

## CI

Workflow: `.github/workflows/dotnet.yml`.

CI виконує:

- `dotnet restore`;
- `dotnet build --configuration Release`;
- `dotnet test` з coverlet;
- line coverage gate 85%;
- branch coverage gate 80%;
- генерацію HTML coverage report;
- upload `coverage-report` artifact.

Локальний еквівалент CI перевірено: build succeeded, tests passed, coverage threshold passed.
