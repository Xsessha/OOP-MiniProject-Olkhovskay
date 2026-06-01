# Syllabus Coverage

## Матриця

| Група тем | Використано обов'язково | Частково | Розширення фінального етапу |
| --- | --- | --- | --- |
| Основи ООП | `Car`, `Rental`, `Customer`, інкапсуляція стану, методи `Rent()`/`Return()` | - | Уточнено доменні правила в report/docs |
| Абстракції, поліморфізм, інтерфейси | Абстрактний `Customer`, `EconomyCustomer`, `PremiumCustomer`, repository interfaces | - | Знижка в `RentalService` тепер іде через `Customer.GetDiscount()` |
| Generics, колекції, LINQ, делегати | `JsonDataStore<T>`, `Result<T>`, `QueryCache<TKey, TValue>`, `List<T>`, `Dictionary`, `HashSet`, LINQ `Where`, `Sum`, `Average`, `GroupBy`, `OrderByDescending`, delegate pipeline `RentalQuery` | - | Делегати та cache добудовано у самостійній роботі №29 |
| Обробка помилок і persistence | Domain exceptions, `Result.Fail`, JSON load/save, corrupted file handling | File locking відсутній | CLI hardening для невалідного GUID/days |
| SOLID | SRP через шари, DIP через repository interfaces, ISP через read/write interfaces | OCP для нових customer types потребує зміни factory | Розширено developer guide правилами змін |
| Патерни | Factory, Repository, Facade, Observer, Result-like pattern | Strategy як окрема папка не є головною runtime-точкою | Observer і Facade явно винесені в release/demo docs |
| UML | Class, sequence, use-case, facade diagrams | - | UML оновлено під v1.0.0 |
| Тестування | xUnit, unit/integration tests, coverage gates | Console input без E2E automation | Додано тести для discount consistency і customer type normalization |
| Рефакторинг | Pricing config, date provider, read/write interfaces з попередньої ітерації | - | Усунено дублювання знижки, safe CLI parsing, XML comments, analytics pipeline/cache |

## Підсумок

Проєкт покриває основні навчальні теми не лише на рівні списку файлів, а через інтегрований сценарій: користувацький flow проходить через UI, facade, service, domain, repository і persistence. Часткові теми винесені як post-course backlog, щоб scope `v1.0.0` лишився стабільним.
