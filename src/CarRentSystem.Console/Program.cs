using CarRentSystem.Application.Events;
using CarRentSystem.Application.Analytics;
using CarRentSystem.Domain.Entities;
using CarRentSystem.Domain.Exceptions;
using CarRentSystem.Infrastructure.Persistence;
using CarRentSystem.Infrastructure.Repositories;
using CarRentSystem.Application.Services;
using CarRentSystem.Application.Facades;

var filePath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "cars.json"));

ApplicationEventBus.Subscribe(new ConsoleLogger());

var loadResult = JsonDataStore<Car>.LoadResult(filePath);

if (!loadResult.Success)
{
    Console.WriteLine($"[WARNING] Could not load persisted car data: {loadResult.ErrorMessage}. Starting with default fleet.");
}

var cars = loadResult.Success
    ? loadResult.Value!
    : new List<Car>
    {
        new Car("BMW X5"),
        new Car("Audi A6"),
        new Car("Toyota Camry"),
        new Car("Mercedes-Benz S-Class"),
        new Car("Tesla Model 3"),
        new Car("Honda Civic"),
        new Car("Ford Mustang"),
        new Car("Volkswagen Golf"),
        new Car("Porsche 911"),
        new Car("Nissan Rogue"),
        new Car("Hyundai Tucson"),
        new Car("Kia Sportage"),
        new Car("Volvo XC90"),
        new Car("Mazda CX-5"),
        new Car("Subaru Outback"),
        new Car("Lexus RX 350"),
        new Car("Chevrolet Camaro"),
        new Car("Jaguar F-Type")
    };

if (!File.Exists(filePath))
{
    var initSaveResult = JsonDataStore<Car>.Save(filePath, cars);
    if (!initSaveResult.Success)
    {
        Console.WriteLine($"[ERROR] Could not persist initial fleet: {initSaveResult.ErrorMessage}");
    }
}

var carRepo = new CarRepository(cars);
var rentalRepo = new InMemoryRentalRepository();
var service = new RentalService(carRepo, rentalRepo);
var facade = new RentalFacade(service, carRepo, rentalRepo);

while (true)
{
    Console.WriteLine("\n====================================");
    Console.WriteLine("           CAR RENT SYSTEM          ");
    Console.WriteLine("====================================");
    Console.WriteLine("1.  Rent car");
    Console.WriteLine("2.  Return car");
    Console.WriteLine("3.  Show cars");
    Console.WriteLine("4.  Analytics");
    Console.WriteLine("0.  Exit");
    Console.WriteLine("====================================");
    Console.Write("Choose: ");

    var option = Console.ReadLine();

    if (option == "0") break;

    if (option == "1")
    {
        Console.WriteLine("\nAVAILABLE CARS:");
        PrintTable(facade.GetAvailableCars());

        Console.Write("Name: ");
        var name = Console.ReadLine();

        Console.Write("Type (economy/premium): ");
        var type = Console.ReadLine();

        if (!TryReadGuid("Car ID", out var id))
            continue;

        if (!TryReadPositiveInt("Days", out var days))
            continue;

        try
        {
            var result = facade.Rent(name!, type!, id, days);
            var saveResult = JsonDataStore<Car>.Save(filePath, facade.GetCars());

            if (!saveResult.Success)
            {
                Console.WriteLine($"[ERROR] Unable to persist rental state: {saveResult.ErrorMessage}");
            }

            Console.WriteLine($"Customer: {result.Rental.Customer.Name} ({result.CustomerType})");
            Console.WriteLine($"Car: {result.Rental.Car.Model}");
            Console.WriteLine($"Base price: {result.BasePrice:C2}");
            Console.WriteLine($"Final price after discount: {result.DiscountedPrice:C2}");
            Console.WriteLine("RENT SUCCESS");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"Operation failed: {ex.Message}");
            ApplicationEventBus.Notify($"Business failure while renting: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            ApplicationEventBus.Notify($"Unexpected error while renting: {ex}");
        }
    }

    if (option == "2")
    {
        if (!TryReadGuid("Car ID", out var id))
            continue;

        try
        {
            var result = facade.Return(id);
            var saveResult = JsonDataStore<Car>.Save(filePath, facade.GetCars());

            if (!saveResult.Success)
            {
                Console.WriteLine($"[ERROR] Unable to persist return state: {saveResult.ErrorMessage}");
            }

            Console.WriteLine($"Car returned: {result.Rental.Car.Model}");

            if (result.IsLate)
            {
                Console.WriteLine($"Late penalty: {result.Penalty:C2}");
            }
            else
            {
                Console.WriteLine("Returned on time. No penalty.");
            }

            Console.WriteLine($"Total cost: {result.TotalCost:C2}");
        }
        catch (DomainException ex)
        {
            Console.WriteLine($"Operation failed: {ex.Message}");
            ApplicationEventBus.Notify($"Business failure while returning: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            ApplicationEventBus.Notify($"Unexpected error while returning: {ex}");
        }
    }

    if (option == "3")
    {
        Console.WriteLine("\nALL CARS:");
        PrintTable(facade.GetCars());
    }

    if (option == "4")
    {
        Console.WriteLine("\n=== ANALYTICS ===");
        var report = facade.GetAnalyticsReport();
        Console.WriteLine($"Total Revenue: {report.TotalRevenue:C2}");
        Console.WriteLine($"Rentals: {report.RentalCount}");
        Console.WriteLine($"Active rentals: {report.ActiveRentalCount}");
        Console.WriteLine($"Average rental days: {report.AverageRentalDays:F1}");

        if (report.ModelStatistics.Any())
        {
            Console.WriteLine("\nTop 5 Most Rented Cars:");
            foreach (var stat in report.ModelStatistics.Take(5))
            {
                Console.WriteLine($"{stat.Model}: {stat.RentalCount} rentals, {stat.Revenue:C2} revenue");
            }

            var activeReport = facade.GetAnalyticsReport(RentalQuery.All.ActiveOnly());
            Console.WriteLine($"\nActive-rental report: {activeReport.RentalCount} active records.");
        }
        else
        {
            Console.WriteLine("\nNo rentals yet.");
        }
    }
}

void PrintTable(List<Car> cars)
{
    Console.WriteLine("--------------------------------------------------------------------------------");
    Console.WriteLine("ID                                   | MODEL               | PRICE    | STATUS");
    Console.WriteLine("--------------------------------------------------------------------------------");

    foreach (var c in cars)
    {
        Console.WriteLine($"{c.Id} | {c.Model,-18} | {c.PricePerDay,8:C2} | {(c.IsAvailable ? "Available" : "Rented")}");
    }

    Console.WriteLine("--------------------------------------------------------------------------------");
}

bool TryReadGuid(string label, out Guid value)
{
    Console.Write($"{label}: ");
    var input = Console.ReadLine();

    if (Guid.TryParse(input, out value))
        return true;

    Console.WriteLine($"{label} must be a valid GUID.");
    value = Guid.Empty;
    return false;
}

bool TryReadPositiveInt(string label, out int value)
{
    Console.Write($"{label}: ");
    var input = Console.ReadLine();

    if (int.TryParse(input, out value) && value > 0)
        return true;

    Console.WriteLine($"{label} must be a positive number.");
    value = 0;
    return false;
}
