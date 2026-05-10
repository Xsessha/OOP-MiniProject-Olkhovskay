# TESTING.md

## Як запускати тести

Запуск усіх тестів:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj
```

Запуск у Release, як у CI:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release --no-restore
```

Запуск з coverage gate 85% для line coverage:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults\coverage\ /p:Threshold=85 /p:ThresholdType=line /p:ThresholdStat=total
```

Генерація HTML-звіту:

```bash
reportgenerator -reports:tests/MyProject.Tests/TestResults/coverage/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html
```

Результат відкривається у:

```text
coverage-report/index.html
```

## Що покрито тестами

Unit tests покривають:

- доменні інваріанти `Car`, `Rental`, `Customer`, `Money`;
- boundary values для днів оренди, цін і порожніх значень;
- `RentalService` і domain exceptions;
- `CustomerFactory` як точку розширення створення клієнтів;
- `RentalFacade`;
- `RentalAnalyticsService`;
- `Result` / `Result<T>`;
- event listeners і event bus;
- `FileStorage`, `JsonService`, `JsonDataStore<T>`.

Integration tests покривають:

- створення даних;
- збереження в тимчасовий JSON-файл;
- повторне завантаження;
- оренду після відновлення стану;
- повернення після відновлення стану;
- кілька послідовних операцій;
- missing file;
- corrupted JSON;
- invalid save path;
- логування persistence errors.

Fault handling tests покривають:

- `CarNotFoundException`;
- `CarAlreadyRentedException`;
- `RentalNotFoundException`;
- `InvalidCustomerTypeException`;
- `RentalLimitExceededException`;
- `ArgumentException` для невалідних доменних значень;
- `Result.Fail` для persistence failures;
- graceful fallback для missing/corrupted files.

## Поточні метрики

Останній перевірений запуск:

- 212 xUnit test cases passed;
- 168 unit test methods;
- 25 integration test methods;
- line coverage: 89.87%;
- branch coverage: 88.04%;
- method coverage: 88.88%;
- 0 класів з 0% executable line coverage.

## CI quality gate

Workflow `.github/workflows/dotnet.yml` виконує:

- `dotnet restore`;
- `dotnet build --configuration Release`;
- `dotnet test` з coverlet;
- line coverage threshold 85%;
- branch coverage threshold 80%;
- HTML report generation;
- перевірку наявності `coverage-report/index.html`;
- upload `coverage-report` artifact.

