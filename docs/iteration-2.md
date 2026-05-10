# Iteration 2 Summary - Lab 35

## Реалізовані use cases

1. Оренда автомобіля:
   - пошук доступного авто;
   - створення клієнта через factory;
   - застосування правил economy/premium;
   - створення запису оренди;
   - зміна стану авто.

2. Повернення автомобіля:
   - пошук активної оренди;
   - повернення авто в доступний стан;

3. Перегляд автомобілів:
   - список усіх авто;
   - фільтр доступних авто;
   - відображення ціни та статусу.

4. Аналітика оренди:
   - загальний дохід;
   - популярність моделей;
   - активні оренди;
   - унікальні клієнти.

## Архітектурні елементи

- Domain: `Car`, `Rental`, `Customer`, `EconomyCustomer`, `PremiumCustomer`, `Money`.
- Application: `RentalService`, `RentalFacade`, `CustomerFactory`, events, analytics.
- Infrastructure: repositories, `JsonDataStore<T>`, `FileStorage`, `JsonService`.
- Console: меню та handlers для сценаріїв користувача.

## Патерни

- Factory: `CustomerFactory`.
- Facade: `RentalFacade`.
- Repository: `ICarRepository`, `IRentalRepository` та in-memory/file-backed реалізації.
- Strategy-like pricing/discount behavior: типи клієнтів і конфігурація цін авто.

## Ризики, передані в Lab 36

- Недостатнє покриття domain edge cases.
- Недостатнє покриття persistence failures.
- Старий hotspot у логіці ціноутворення авто.
- Відсутність сильного coverage gate.
- Неповна документація тестової стратегії.

Lab 36 закриває ці ризики через тестову стратегію, unit/integration tests, fault handling, coverage report і CI quality gate.
