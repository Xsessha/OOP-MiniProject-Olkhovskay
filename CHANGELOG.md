# Changelog

## [1.0.0] - 2026-05-19

### Added

- Повний релізний пакет документації: README, user guide, developer guide, testing guide, final report, demo script.
- `docs/release-plan.md` зі scope релізу, післякурсовими задачами, допустимими боргами та покриттям тем курсу.
- `docs/syllabus-coverage.md` з матрицею тем курсу.
- `docs/defense-qa.md` з питаннями для захисту.
- `docs/performance-analysis.md` з аналізом LINQ, `Dictionary` та `HashSet`.
- Демо-набір даних `docs/demo-cars.json`.
- Тести для узгодженого розрахунку знижки і case-insensitive типу клієнта.

### Changed

- `RentalService` тепер використовує доменний поліморфізм `Customer.GetDiscount()` для розрахунку `DiscountedPrice`.
- Тип клієнта нормалізується перед валідацією та створенням об'єкта.
- Пошук клієнтів в аналітиці використовує `StringComparison.OrdinalIgnoreCase`.
- Console UI безпечно обробляє некоректний GUID і кількість днів.
- UML-артефакти оновлено під поточну структуру шарів.

### Fixed

- Усунуто дублювання правил знижки між `RentalService` і доменними customer classes.
- Виправлено невідповідність: economy-клієнт у `Rental.TotalPrice` мав 5% знижку, а `RentOperationResult.DiscountedPrice` раніше показував базову ціну.
- Запобігано падінню застосунку при невалідному введенні `Car ID` або `Days`.
- Запобігано зміні статусу авто, якщо `RentalService` отримує невалідну кількість днів.
