# Iteration 4 Report - Final Release Hardening

## Мета

Завершити навчальний реліз `v1.0.0`: стабілізувати код, закрити документацію, підготувати демо та показати покриття тем курсу.

## Зроблено

- Уточнено release scope у [release-plan.md](release-plan.md).
- Виконано цільовий рефакторинг у `RentalService`: pricing тепер використовує `Customer.GetDiscount()`.
- Додано нормалізацію типу клієнта і тести для цього сценарію.
- Додано перевірку, що невалідні дні оренди не змінюють стан авто.
- Посилено console UX: невалідний GUID або `Days` більше не завершують застосунок.
- Оновлено README, user guide, developer guide, testing guide, changelog, demo script і final report.
- Додано syllabus coverage, performance analysis, defense Q&A, presentation outline і demo dataset.
- Оновлено UML-артефакти.

## Перевірка

- `dotnet build CarRentSystemstem.sln --configuration Release` - success, 0 warnings, 0 errors.
- `dotnet test tests/CarRentSystem.Tests/CarRentSystem.Tests.csproj --configuration Release` - 217 passed.
- Coverage: 91.34% line, 87.50% branch, 91.97% method.

## Висновок

Проєкт готовий до фінального коміту і тегу `v1.0.0`.
