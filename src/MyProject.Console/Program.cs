using MyProject.Application.Services;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Repositories;
using MyProject.Console.Menu;
using MyProject.Console.Handlers;

var repo = new InMemoryCarRepository();
var service = new RentalService(repo);

var cars = new List<Car>
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

foreach (var car in cars)
{
    repo.Add(car);
}

var handler = new RentCarHandler(service, repo);
var menu = new MenuHandler();

menu.Register(1, handler.Handle);

menu.Run();