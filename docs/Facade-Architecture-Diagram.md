```mermaid
classDiagram

class Console {
    +Menu()
}

class RentalFacade {
    +Rent()
    +Return()
    +GetCars()
    +GetAnalytics()
}

class RentalService
class CarRepository
class RentalRepository

Console --> RentalFacade
RentalFacade --> RentalService
RentalFacade --> CarRepository
RentalFacade --> RentalRepository