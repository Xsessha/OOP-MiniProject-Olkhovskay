```mermaid
classDiagram
    class Car {
        +Guid Id
        +string Brand
        +bool IsAvailable
        +decimal DailyPrice
    }

    class Customer {
        +Guid Id
        +string Name
        +GetDiscount() double
    }

    class PremiumCustomer {
        +double Discount
        +GetDiscount() double
    }

    class EconomyCustomer {
        +int MaxRentals
        +GetDiscount() double
    }

    class Rental {
        +Guid Id
        +DateTime RentDate
        +DateTime ReturnDate
        +CalculateTotal() decimal
    }

    class RentalService {
        -ICarRepository _repository
        +RentCar(string customerName, Guid carId) void
    }

    class ICarRepository {
        <<interface>>
        +Add(Car car) void
        +GetById(Guid id) Car
        +GetAll() List~Car~
    }

    Car <-- Rental
    Customer <|-- PremiumCustomer
    Customer <|-- EconomyCustomer
    Rental --> Customer
    RentalService --> ICarRepository