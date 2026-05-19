# Defense 

## Архітектура

**Чому проєкт поділено на кілька шарів?**  
Щоб відокремити бізнес-правила від console UI та persistence. `Domain` містить сутності, `Application` координує use cases, `Infrastructure` зберігає дані, `Console` тільки взаємодіє з користувачем.

**Навіщо потрібен `RentalFacade`?**  
Він спрощує API для UI. Console не знає деталей repositories, analytics і service methods, а звертається до одного об'єкта.

**Де використано поліморфізм?**  
В абстрактному `Customer`: `EconomyCustomer` і `PremiumCustomer` по-різному реалізують `GetDiscount()`.

**Які патерни використано?**  
Factory, Repository, Facade, Observer та Result-like pattern для persistence.

## Бізнес-логіка

**Чому economy-клієнт має обмеження на 10 днів?**  
Це бізнес-правило, яке демонструє domain validation і окремий exception `RentalLimitExceededException`.

**Що буде при повторній оренді того самого авто?**  
`Car.Rent()` перевіряє `IsAvailable` і кидає `CarAlreadyRentedException`.

**Як рахується фінальна ціна?**  
`RentalService` бере `PricePerDay * days`, а знижку отримує через `Customer.GetDiscount()`. Це прибирає дублювання pricing rules.

## Persistence

**Де зберігаються дані?**  
У `cars.json` в корені репозиторію.

**Що буде, якщо JSON пошкоджено?**  
`JsonDataStore<T>.LoadResult` поверне failure, подія залогується через `ApplicationEventBus`, а застосунок стартує зі стандартним автопарком.

**Чому не база даних?**  
Для навчального capstone достатньо JSON, щоб показати persistence без зайвої інфраструктури. SQL перенесено в post-course scope.

## Тестування

**Скільки тестів проходить?**  
217 tests passed у фінальному локальному прогоні.

**Яке покриття?**  
Total line coverage 91.34%, branch coverage 87.50%, method coverage 91.97%.

**Що саме перевіряють integration tests?**  
Persistence, повний flow оренди/повернення, відновлення з JSON, помилки файлів.

## Рефакторинг

**Який фінальний рефакторинг найважливіший?**  
Усунення дублювання знижки: `DiscountedPrice` тепер використовує той самий поліморфний механізм, що й `Rental.TotalPrice`.

**Чому це важливо?**  
До правки economy-знижка була в домені, але не відображалась у результаті операції оренди. Це була невідповідність між розрахунком і UI.

## Обмеження

**Який головний технічний борг?**  
JSON persistence без locking і transactions.

**Що б ви зробили наступним?**  
Додав би SQL repository, DI container, end-to-end console tests і batch import/export.
