using MyProject.Domain.Entities;
using MyProject.Infrastructure.Persistence;
using MyProject.Infrastructure.Repositories;
using MyProject.Application.Services;
using MyProject.Application.Facades;

var filePath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "cars.json"));

var cars = File.Exists(filePath)
    ? JsonDataStore<Car>.Load(filePath)
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
    JsonDataStore<Car>.Save(filePath, cars);

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

        Console.Write("Car ID: ");
        var id = Guid.Parse(Console.ReadLine()!);

        Console.Write("Days: ");
        var days = int.Parse(Console.ReadLine()!);

        facade.Rent(name!, type!, id, days);
        JsonDataStore<Car>.Save(filePath, facade.GetCars());

        Console.WriteLine(" RENT SUCCESS");
    }

    if (option == "2")
    {
        Console.Write("Car ID: ");
        var id = Guid.Parse(Console.ReadLine()!);

        facade.Return(id);
        JsonDataStore<Car>.Save(filePath, facade.GetCars());

        Console.WriteLine("RETURN SUCCESS");
    }

    if (option == "3")
    {
        PrintTable(facade.GetCars());
    }

    if (option == "4")
    {
        Console.WriteLine("\nANALYTICS");

        Console.WriteLine($"Total revenue: {facade.GetRevenue():C2}");

        Console.WriteLine("\nTop cars:");
        foreach (var g in facade.GetTopCars())
            Console.WriteLine($"{g.Key} -> {g.Count()} rentals");
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