# Car Rent System

Навчальний консольний застосунок для оренди автомобілів. Проєкт показує базові теми ООП-курсу: сутності домену, наслідування, поліморфізм, інтерфейси, репозиторії, JSON persistence, LINQ-аналітику, патерни Factory/Facade/Observer та автоматизоване тестування.

## Можливості

- перегляд автопарку та статусу автомобілів;
- оренда автомобіля для `economy` або `premium` клієнта;
- повернення автомобіля з розрахунком штрафу за прострочення;
- збереження стану автопарку в `cars.json`;
- аналітика: загальний дохід і топ орендованих моделей;
- обробка очікуваних бізнес-помилок через доменні винятки;
- CI quality gate з тестами та coverage.

## Запуск

Потрібен .NET SDK 9.0.

```bash
dotnet restore
dotnet run --project src/MyProject.Console/MyProject.Console.csproj
```

Тести:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release
```

Coverage, як у CI:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release --no-restore /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults\coverage\ /p:Threshold=85 /p:ThresholdType=line /p:ThresholdStat=total
```

## Структура

```text
src/
  MyProject.Domain/          доменні сутності, value objects, exceptions, interfaces
  MyProject.Application/     use cases, facade, analytics, factories, event bus
  MyProject.Infrastructure/  JSON persistence, repositories, serialization
  MyProject.Console/         консольний UI
tests/
  MyProject.Tests/           unit та integration tests
docs/                        UML, release plan, coverage matrix, defense materials
```

## Документація

- [USER_GUIDE.md](USER_GUIDE.md) - сценарії користувача.
- [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) - архітектура та правила розширення.
- [TESTING.md](TESTING.md) - запуск тестів, coverage, CI.
- [DEMO.md](DEMO.md) - сценарій захисту на 3-5 хвилин.
- [CHANGELOG.md](CHANGELOG.md) - зміни релізу.
- [FINAL_REPORT.md](FINAL_REPORT.md) - фінальний технічний звіт.
- [docs/release-plan.md](docs/release-plan.md) - scope v1.0.0 і борги.
- [docs/syllabus-coverage.md](docs/syllabus-coverage.md) - матриця покриття тем курсу.
- [docs/defense-qa.md](docs/defense-qa.md) - питання та короткі відповіді.

## Release

Поточний стан підготовлено як навчальний реліз `v1.0.0`. Після фінального коміту тег створюється командою:

```bash
git tag v1.0.0
git push origin v1.0.0
```
