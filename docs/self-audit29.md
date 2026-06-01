# Self-Audit — Покриття курсу проєктом Car Rent System

> Самостійна робота №29 · CarRentSystem · v1.0.0

---

## Матриця аудиту

| Блок тем | Що знайдено у проєкті | Статус |
|---|---|---|
| **Основи ООП** | `Car`, `Rental`, `Customer` з інкапсуляцією стану; методи `Rent()`/`Return()`; конструктори з валідацією інваріантів; `Money`, `DateRange` як value objects | ✅ Використано впевнено |
| **Абстракції** | Абстрактний клас `Customer`; `EconomyCustomer`, `PremiumCustomer` як конкретні реалізації; `ICarRepository`, `IRentalRepository`, `IReadRepository<T>`, `IWriteRepository<T>` | ✅ Використано впевнено |
| **Колекції та generics** | `JsonDataStore<T>` — generic persistence; `List<T>` в репозиторіях; `Dictionary<string, int>` для підрахунку моделей; `HashSet<string>` для унікальних клієнтів | ✅ Використано впевнено |
| **Делегати та лямбда-вирази** | Делегати є в `MenuHandler`; лямбди в LINQ-запитах; але окремого `Func<T>` / `Predicate<T>` pipeline для фільтрації немає — це точка росту | ⚠️ Використано частково |
| **LINQ** | `Where`, `Sum`, `GroupBy`, `OrderByDescending`, `FirstOrDefault`, `Select` в `RentalAnalyticsService`; join відсутній | ⚠️ Використано частково |
| **Обробка помилок** | Domain exceptions (`CarNotAvailableException`, `RentalNotFoundException`); `Result<T>` для persistence failures; hardening CLI для некоректного GUID/днів | ✅ Використано впевнено |
| **SOLID** | SRP через шари Domain/Application/Infrastructure/Console; DIP через repository interfaces; ISP через `IReadRepository<T>` / `IWriteRepository<T>`; OCP частково (нові customer types вимагають зміни Factory) | ⚠️ Використано частково |
| **Патерни** | Factory (`CustomerFactory`), Repository, Facade (`RentalFacade`), Observer (`ApplicationEventBus`), Result-like pattern; Strategy не є явною runtime-точкою | ✅ Використано впевнено |
| **UML і документація** | Class diagram, sequence diagram, use-case diagram, facade diagram; release-plan, defense-qa, syllabus-coverage, DEMO, FINAL\_REPORT, CHANGELOG, DEVELOPER\_GUIDE | ✅ Використано впевнено |
| **Тестування** | xUnit, 217 тестів (unit + integration), Moq для mock-репозиторіїв, coverage 91.34% lines / 87.50% branches; CI quality gate | ✅ Використано впевнено |
| **Рефакторинг** | Усунено дублювання знижки; нормалізація customer type; перейменування методів; pricing config як окремий об'єкт; `DateProvider` для ін'єкції часу | ✅ Використано впевнено |

---

## Висновок

### Теми покриті найкраще (3–5)

1. **Тестування** — 217 тестів, CI gate, coverage >91%, мок-репозиторії через Moq.
2. **Архітектура і патерни** — чотири шари, Factory/Facade/Observer/Repository в реальному сценарії, не для галочки.
3. **Обробка помилок** — domain exceptions + Result<T> + hardening UI — повний стек.
4. **ООП та абстракції** — abstract Customer з поліморфним GetDiscount(), repository interfaces з ISP.
5. **UML і документація** — всі типи діаграм присутні, DEMO/FINAL\_REPORT/DEVELOPER\_GUIDE оновлені.

### Теми, що хочу добудувати в цій самостійній роботі (3)

1. **Делегати** — додати `ICarFilter` з підтримкою `Func<Car, bool>` pipeline замість ad-hoc лямбд.
2. **LINQ — розширені запити** — додати `SearchCars` з composable-фільтром і розширену аналітику з join-проєкцією.
3. **Параметризовані тести** — покрити новий pipeline Theory-тестами з `[MemberData]`.

### Чому саме ці теми посилять проєкт

Делегат-фільтр — це справжня точка розширення: нові умови пошуку не потребуватимуть змін в `RentalFacade`. LINQ join покаже вміння будувати проєкції між двома доменними колекціями, якого зараз немає. Параметризовані тести зафіксують поведінку pipeline для граничних кейсів і підвищать branch coverage. Разом вони утворюють ланцюжок: фільтр → аналітика → тест, де кожен крок залежить від попереднього.