# Demo Script

## Підготовка

Для повністю повторюваного демо можна відновити стартовий набір:

```powershell
Copy-Item docs\demo-cars.json cars.json -Force
```

Запуск:

```bash
dotnet run --project src/MyProject.Console/MyProject.Console.csproj
```

## Сценарій

### 1. Старт застосунку

Показати головне меню. Коротко пояснити, що console UI є тонким шаром, а бізнес-логіка знаходиться в `RentalService`.

### 2. Перегляд даних

Обрати `3. Show cars`.

Очікування: у таблиці видно автомобілі, їх GUID, ціну та статус.

### 3. Ключовий сценарій: оренда

Обрати `1. Rent car`.

Дані з `docs/demo-cars.json`:

```text
Name: Olena
Type: premium
Car ID: 33333333-3333-3333-3333-333333333333
Days: 3
```

Очікування:

- авто `Tesla Model 3` стає `Rented`;
- base price: 450;
- final price: 360;
- показано `RENT SUCCESS`.

### 4. Негативний сценарій

Знову обрати `1. Rent car`.

```text
Name: Test User
Type: vip
Car ID: 11111111-1111-1111-1111-111111111111
Days: 2
```

Очікування: повідомлення `Operation failed: Invalid customer type...`, застосунок не завершується.

Додатково можна ввести некоректний GUID і показати, що CLI тепер відповідає `Car ID must be a valid GUID`.

### 5. Збереження та відновлення

Обрати `0. Exit`, запустити застосунок ще раз і обрати `3. Show cars`.

Очікування: `Tesla Model 3` лишається `Rented`, бо стан збережено у `cars.json`.

### 6. Повернення

Обрати `2. Return car`.

```text
Car ID: 33333333-3333-3333-3333-333333333333
```

Очікування: авто повернено, штраф відсутній, вартість показано.

### 7. Аналітика

Обрати `4. Analytics`.

Пояснити, що агрегації виконуються через LINQ і repository abstraction.

## Архітектурне пояснення для захисту

- `Program.cs` тільки збирає залежності та читає введення.
- `RentalFacade` приховує деталі сервісів від UI.
- `RentalService` координує бізнес-сценарії.
- `Car`, `Rental`, `Customer` містять доменні правила.
- `JsonDataStore<T>` відповідає за persistence і повертає `Result`, щоб контрольовано обробляти I/O failures.
- `ApplicationEventBus` демонструє Observer для повідомлень про помилки та події.
