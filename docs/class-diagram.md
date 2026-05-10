```mermaid
classDiagram

class Car {
    +Guid Id
    +string Model
    +decimal PricePerDay
    +bool IsAvailable
    +Rent()
    +Return()
}

class Customer {
    +string Name
    +GetDiscount() double
}

class PremiumCustomer
class EconomyCustomer

Customer <|-- PremiumCustomer
Customer <|-- EconomyCustomer

class Rental {
    +Guid Id
    +DateTime RentedAt
    +int Days
    +decimal TotalPrice
    +decimal LatePenalty
    +CalculatePenalty()
    +GetTotalCost()
}

class RentalService {
    +RentCar()
    +ReturnCar()
    +GetAvailableCars()
}

class RentalFacade {
    +Rent()
    +Return()
    +GetRevenue()
    +GetTopCars()
    +GetCars()
}

class ICarRepository {
    <<interface>>
    +Add(Car car)
    +GetById(Guid id) Car
    +GetAll() List~Car~
    +Update(Car car)
}

class IRentalRepository {
    <<interface>>
    +Add(Rental rental)
    +GetAll() List~Rental~
}

Car <-- Rental
Customer <|-- PremiumCustomer
Customer <|-- EconomyCustomer

RentalService --> ICarRepository
RentalService --> IRentalRepository
RentalFacade --> RentalService