# Release Plan v1.0.0

Дата підготовки: 2026-05-19.

## Scope v1.0.0

У реліз входить:

- консольний сценарій оренди авто;
- повернення авто з розрахунком штрафу за прострочення;
- перегляд усіх авто та доступних авто;
- economy/premium клієнти з поліморфним розрахунком знижки;
- JSON persistence у `cars.json`;
- graceful handling для missing/corrupted JSON;
- доменні exceptions для очікуваних бізнес-помилок;
- `RentalFacade` як єдина точка для UI;
- аналітика доходу і топ моделей;
- Observer-based event logging через `ApplicationEventBus`;
- unit та integration tests;
- CI workflow з coverage gates;
- фінальна документація, UML, demo script, defense Q&A.

## Після курсу

Переноситься в post-course backlog:

- SQL database або ORM замість JSON;
- file locking або transactions для паралельного доступу;
- Web API або desktop/web UI;
- автентифікація та ролі;
- property-based tests через FsCheck;
- end-to-end tests саме для console input;
- configurable retry policy замість фіксованого retry;
- асинхронний batch import/export;
- розширена аналітика з експортом звітів.

## Допустимі технічні борги

| Борг | Чому допустимо для v1.0.0 | Ризик |
| --- | --- | --- |
| JSON-файл без locking | Навчальний single-user сценарій | Конфлікти при паралельному запуску |
| Static `ApplicationEventBus` | Простий Observer без DI container | Складніше ізолювати global state у великих тестах |
| In-memory repositories | Достатньо для малого автопарку | Потрібна заміна при великому обсязі даних |
| Console UI без E2E input tests | Бізнес-логіка покрита сервісними тестами | Не всі CLI edge cases автоматизовані |
| Немає deployment job | Це навчальний реліз, не production delivery | Релізний пакет готується вручну |

## Покриття тем курсу перед релізом

Повністю покрито:

- основи ООП: класи, інкапсуляція, наслідування;
- абстракції, інтерфейси, поліморфізм;
- collections, generics, LINQ;
- exceptions і persistence;
- SOLID на рівні шарів і контрактів;
- Factory, Facade, Repository, Observer;
- UML class/sequence/use-case diagrams;
- unit та integration testing;
- targeted refactoring.

Частково покрито:

- advanced async workflows;
- production-grade resilience;
- UI automation;
- database design.

Фінальні розширення для закриття прогалин:

- Observer для повідомлень і persistence failures;
- Facade для простого API системи;
- LINQ-аналітика з `Dictionary` і `HashSet`;
- явніше використання абстрактного базового класу `Customer`.

## Release checklist

- [x] `dotnet build MyProject.sln --configuration Release`
- [x] `dotnet test tests/MyProject.Tests/MyProject.Tests.csproj --configuration Release`
- [x] coverage threshold 85% line пройдено
- [x] branch coverage вище 80%
- [x] README посилається на решту документації
- [x] demo script створено
- [x] syllabus coverage створено
- [x] UML оновлено
- [ ] фінальний commit
- [ ] `git tag v1.0.0`
- [ ] `git push origin v1.0.0`
