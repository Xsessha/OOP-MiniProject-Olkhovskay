# Testing Guide

Основний файл з інструкціями для запуску тестів: [`../TESTING.md`](../TESTING.md).

Коротко:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj
```

Coverage:

```bash
dotnet test tests/MyProject.Tests/MyProject.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=TestResults\coverage\
```

HTML report:

```bash
reportgenerator -reports:tests/MyProject.Tests/TestResults/coverage/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html
```

Актуальний звіт: `coverage-report/index.html`.
