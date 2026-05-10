# Test Strategy для Lab 36

## Мета

Lab 36 перевіряє, що система оренди автомобілів витримує не тільки щасливі сценарії, а й помилки введення, збої persistence-шару, регресії в доменній логіці та зміну вимог. Тестування побудоване як test pyramid: більшість правил перевіряється швидкими unit-тестами, а файлове збереження і повні сценарії підтверджуються інтеграційними тестами.

## Критичні сценарії

1. Оренда доступного автомобіля:
   - пошук автомобіля за `Id`;
   - перевірка доступності;
   - зміна стану `Car.IsAvailable`;
   - створення `Rental`;
   - розрахунок ціни з урахуванням типу клієнта.

2. Заборонені переходи стану:
   - повторна оренда вже зайнятого авто;
   - повернення неіснуючого автомобіля;
   - повернення автомобіля без активної оренди;
   - некоректна кількість днів оренди.

3. Доменні інваріанти:
   - `Car` не може мати порожню модель;
   - `Customer` не може мати порожнє ім'я;
   - `Rental` не створюється з `null` залежностями або днями поза межами;
   - `Money` не приймає від'ємні значення.

4. Persistence і fault handling:
   - збереження JSON;
   - повторне завантаження стану;
   - missing file повертає порожню колекцію;
   - corrupted JSON не валить застосунок;
   - I/O помилки повертають `Result.Fail` і логуються через event bus.

5. Аналітика:
   - active rentals;
   - пошук клієнтів;
   - top rented cars;
   - total revenue;
   - порожні колекції не спричиняють падіння LINQ-запитів.

## Найважчі частини для тестування

Persistence-шар найважчий, бо залежить від файлової системи, JSON-серіалізації та помилок I/O. Для нього використовуються тимчасові файли в інтеграційних тестах.

`RentalService` має кілька залежностей і змінює стан автомобілів та репозиторіїв. Для контрольованих тестів використано in-memory repositories і `IDateTimeProvider`, щоб прибрати приховану залежність від поточного часу.

Консольне логування є побічним ефектом. Його перевірено окремо через перехоплення `Console.Out`, а бізнес-логіка не залежить від консолі напряму.

## Де потрібні mocks або seams

- `ICarReadRepository` і `ICarWriteRepository` потрібні для ізоляції читання та запису авто.
- `IRentalReadRepository` і `IRentalWriteRepository` потрібні для ізоляції історії оренд.
- `IDateTimeProvider` потрібен для стабільної перевірки штрафів і прострочення.
- `IEventListener` потрібен для тестування логування помилкових сценаріїв без реального UI.

У тестах переважно використано in-memory реалізації замість Moq, бо вони простіші, швидші та достатні для поточного рівня складності.

## Де потрібна реальна інтеграція

Реальна інтеграція потрібна для:

- `JsonDataStore<T>`;
- `FileStorage`;
- save/load циклів;
- відновлення стану після перезапуску;
- corrupted/missing file scenarios;
- повного циклу rent -> save/load -> return.

Ці сценарії не мокаються, бо саме взаємодія з файловою системою є ризиком.

## Негативні сценарії перед захистом

- оренда неіснуючого авто;
- повторна оренда вже зайнятого авто;
- некоректний тип клієнта;
- перевищення ліміту оренди для economy-клієнта;
- `Rental` з `null` car/customer;
- `Money` з від'ємною сумою;
- corrupted JSON;
- missing file;
- invalid save path;
- порожні репозиторії та порожні колекції в аналітиці.

## Цільовий рефакторинг для тестованості

Перед розширенням тестів виконано невеликий рефакторинг:

- repository interfaces розділені на read/write контракти за ISP;
- поточний час винесено в `IDateTimeProvider`;
- операції rent/return повертають operation result objects;
- ціноутворення моделей авто винесено з великого `Car.GetDefaultPrice()` у `CarPricingConfiguration` з dictionary mapping;
- persistence-помилки представлені через `Result`/`Result<T>`;
- event listeners винесені за `IEventListener`.

Це зменшило приховані залежності та зробило критичні сценарії контрольованими в тестах.

## Стратегія покриття

- Unit tests: доменні інваріанти, `RentalService`, factory/facade, analytics, result wrappers, events.
- Integration tests: persistence, save/load, rent after restore, full flow, corrupted/missing files.
- Fault handling tests: доменні винятки, invalid path, corrupted JSON, event logging.
- Theory tests: boundary values, invalid values, repeated model/customer cases.

## Quality gate

CI виконує:

- restore;
- Release build;
- `dotnet test` з coverlet;
- line coverage gate 85%;
- branch coverage gate 80%;
- HTML report generation в `coverage-report/index.html`;
- upload coverage report як artifact.

Актуальний локальний результат після Lab 36:

- 212 xUnit test cases passed;
- line coverage: 89.87%;
- branch coverage: 88.04%;
- 0 класів з 0% executable line coverage.
