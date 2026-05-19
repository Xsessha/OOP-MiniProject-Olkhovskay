# Facade Architecture Diagram

```mermaid
flowchart LR
    Console[Console UI]
    Facade[RentalFacade]
    Service[RentalService]
    Analytics[RentalAnalyticsService]
    CarRepo[ICarRepository]
    RentalRepo[IRentalRepository]
    Domain[Domain Entities]
    Json[JsonDataStore]

    Console --> Facade
    Console --> Json
    Facade --> Service
    Facade --> CarRepo
    Facade --> RentalRepo
    Analytics --> CarRepo
    Analytics --> RentalRepo
    Service --> CarRepo
    Service --> RentalRepo
    Service --> Domain
```
