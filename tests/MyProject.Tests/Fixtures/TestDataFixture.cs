using MyProject.Domain.Entities;

namespace MyProject.Tests.Fixtures;

public static class TestDataFixture
{
    public static Car CreateCar(string model = "BMW")
    {
        return new Car(model);
    }

    public static EconomyCustomer CreateEconomyCustomer(string name = "John")
    {
        return new EconomyCustomer(name);
    }

    public static PremiumCustomer CreatePremiumCustomer(string name = "Alex")
    {
        return new PremiumCustomer(name);
    }

    public static Rental CreateRental(int days = 3)
    {
        var car = CreateCar();
        var customer = CreateEconomyCustomer();

        return new Rental(car, customer, days);
    }
}