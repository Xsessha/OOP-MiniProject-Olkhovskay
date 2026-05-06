
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
        Service->>Car: Rent() / IsAvailable = false
        activate Car
        Car-->>Service: Status updated
        deactivate Car
        Service-->>Console: Return Success
    else Car not found / unavailable
        Service-->>Console: Return Error
    end
    
    deactivate Service

    Console-->>User: Display final result
    deactivate Console