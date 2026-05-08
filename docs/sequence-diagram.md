
```mermaid
sequenceDiagram
    autonumber
    actor User
    participant Console as Console (CLI)
    participant Service as RentalService
    participant Repo as ICarRepository
    participant Car as Car Entity

    User->>Console: Input Name & Car ID
    activate Console

    Console->>Service: RentCar(name, carId)
    activate Service

    Service->>Repo: GetById(carId)
    activate Repo
    Repo-->>Service: Return Car object
    deactivate Repo

    alt Car is available
        Service->>Car: Rent()
        activate Car
        Car-->>Service: Updated state
        deactivate Car

        Service-->>Console: Success
    else Car not found / unavailable
        Service-->>Console: Error
    end

    deactivate Service
    Console-->>User: Result shown
    deactivate Console