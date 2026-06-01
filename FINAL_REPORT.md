# Final Report

## Мета

Підготувати навчальний реліз `v1.0.0` для console-застосунку оренди авто: стабілізувати код, оновити документацію, підтвердити тести, показати покриття тем курсу і підготувати матеріали для захисту.

## Що реалізовано

Проєкт має повний базовий цикл:

- перегляд автопарку;
- оренда авто для economy/premium клієнта;
- повернення авто;
- розрахунок знижки та штрафу;
- збереження стану в JSON;
- аналітика доходу та популярності моделей;
- delegate pipeline для фільтрації оренд;
- кешований LINQ-звіт для демо та аналітичних запитів;
- автоматизовані unit/integration tests;
- CI workflow з coverage gates.

## Архітектурні рішення

Код поділено на шари `Domain`, `Application`, `Infrastructure`, `Console`. Це зменшує зв'язність: console не вирішує бізнес-правила, infrastructure не впливає на домен, а application працює через interfaces.

Основні патерни:

- Factory для створення клієнтів.
- Facade для спрощення API для UI.
- Repository для доступу до авто та оренд.
- Observer для системних повідомлень.
- Result для контрольованих persistence failures.

## Фінальний рефакторинг

Перед релізом виконано невеликий, але помітний hardening:

- прибрано дублювання правил знижки в `RentalService`: тепер `DiscountedPrice` рахується через `Customer.GetDiscount()`;
- нормалізовано тип клієнта (`" Premium "` -> `premium`) перед валідацією;
- виправлено невідповідність, коли economy-знижка була в `Rental.TotalPrice`, але не була в `RentOperationResult.DiscountedPrice`;
- додано XML-коментарі до public API сервісів, фабрики та аналітики;
- `RentalAnalyticsService.SearchByCustomer` переведено з `ToLower()` на `StringComparison.OrdinalIgnoreCase`;
- console UI більше не падає на некоректному GUID або кількості днів;
- `RentalService` валідовує діапазон днів до зміни стану автомобіля.

## Самостійна робота №29

Після аудиту покриття курсу добудовано три залежні розширення:

- `RentalQuery` - delegate/lambda pipeline для складання фільтрів оренд;
- `RentalAnalyticsReport` - розширений LINQ-звіт над результатом `RentalQuery`;
- `QueryCache<TKey, TValue>` - generic cache для повторюваних аналітичних звітів.

Ланцюжок залежностей виглядає так: фільтр формує набір оренд, LINQ-звіт агрегує саме цей набір, cache зберігає результат для стабільного `RentalQuery.CacheKey`. `RentalFacade` очищає cache після `Rent` і `Return`, щоб demo-звіт не показував застарілі дані.

Обов'язкові артефакти самостійної роботи:

- [docs/self-audit.md](docs/self-audit.md)
- [docs/extension-plan.md](docs/extension-plan.md)
- [docs/extension-report.md](docs/extension-report.md)
- [docs/defense-checklist.md](docs/defense-checklist.md)

## Продуктивність і структури даних

Критичний сценарій для аналізу: аналітика оренд.

- `List<T>` доречний для малого навчального автопарку і простого in-memory repository.
- `Dictionary<string, int>` використовується для підрахунку популярності моделей.
- `HashSet<string>` використовується для унікальних клієнтів.
- `QueryCache<TKey, TValue>` використовує `Dictionary<TKey, TValue>` для повторних звітів.
- LINQ застосовується для фільтрації, сортування та агрегації.

Поточний обсяг даних малий, тому додатковий індекс за `CarId` не потрібен. Для production-обсягу наступним кроком був би cache або repository на базі БД.

Деталі: [docs/performance-analysis.md](docs/performance-analysis.md).

## Тестування

Останній локальний прогін: 2026-05-21.

- build: succeeded, 0 warnings, 0 errors;
- tests: 220 passed, 0 failed;
- total line coverage: 91.06%;
- total branch coverage: 83.62%;
- total method coverage: 92.39%.

## Компроміси

- Persistence лишається JSON-файлом без транзакцій і file locking.
- `ApplicationEventBus` має static state, що прийнятно для навчального console app, але не для великої production-системи.
- Console UI покритий через сервіси, а не повноцінними end-to-end input tests.
- Немає реальної авторизації, ролей і багатокористувацького режиму.

## Поза scope

- Web API або GUI.
- SQL database та migrations.
- Повна система бронювання з календарем.
- Асинхронний batch import/export.
- Property-based testing через FsCheck.

## Release readiness

Репозиторій підготовлено до `v1.0.0`: код збирається, тести проходять, coverage gate пройдено, документацію та демо-матеріали оновлено. Тег `v1.0.0` варто створити після фінального коміту.
