# Performance Analysis

## Обраний критичний сценарій

Сценарій: побудова аналітики оренд.

Методи:

- `RentalAnalyticsService.GetCarPopularity()`;
- `RentalAnalyticsService.GetUniqueCustomers()`;
- `RentalAnalyticsService.GetTotalRevenue()`;
- `RentalFacade.GetTopCars()`.

Це доречний сценарій для аналізу, бо він читає всі оренди, групує дані та готує підсумки для demo/defense.

## Структури даних

| Структура | Де використано | Чому доречно |
| --- | --- | --- |
| `List<Car>` | repositories, table output | Малий автопарк, просте додавання та ітерація |
| `List<Rental>` | `InMemoryRentalRepository` | Послідовна історія оренд, проста агрегація |
| `Dictionary<string, int>` | `GetCarPopularity()` | O(1) average update для лічильника моделі |
| `HashSet<string>` | `GetUniqueCustomers()` | Автоматичне усунення дублікатів, O(1) average lookup |
| LINQ `GroupBy`, `Sum`, `Where` | facade та analytics | Читабельні декларативні запити для малого набору |

## Мікроаналіз складності

Нехай `n` - кількість оренд, `m` - кількість авто.

| Операція | Складність | Коментар |
| --- | ---: | --- |
| Пошук авто за ID у `List<Car>` | O(m) | Для стандартного автопарку з 18 авто достатньо |
| Фільтрація доступних авто | O(m) | Прохід по автопарку |
| Загальний дохід `Sum` | O(n) | Один прохід по rentals |
| Популярність моделей через `Dictionary` | O(n) average | Один прохід, оновлення лічильника |
| Унікальні клієнти через `HashSet` | O(n) average | Один прохід, без додаткової ручної перевірки |
| Топ моделей через `GroupBy` + sort | O(n + k log k) | `k` - кількість різних моделей |

## Що оптимізовано

- У `RentalService` прибрано дублювання pricing logic: ціна тепер використовує `Customer.GetDiscount()`.
- У `SearchByCustomer` замінено `ToLower().Contains(...)` на `Contains(..., StringComparison.OrdinalIgnoreCase)`, щоб не створювати проміжні lowercase strings.
- Для унікальних клієнтів використано `HashSet`, а не ручну перевірку `List.Contains`.
- Для популярності моделей лишено `Dictionary`, бо це найпростіший і доречний лічильник.

## Чи потрібні додаткові зміни

Для навчального `v1.0.0` додаткові оптимізації не потрібні. Обсяг даних малий, а код лишається простим для захисту.

Для production-версії можна додати:

- індекс `Dictionary<Guid, Car>` у repository для O(1) пошуку авто;
- database queries замість in-memory LINQ;
- cache для топ моделей;
- pagination для великих списків авто.
