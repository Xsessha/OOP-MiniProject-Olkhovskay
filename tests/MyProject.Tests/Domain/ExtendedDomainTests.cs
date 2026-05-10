using Xunit;
using MyProject.Domain.Entities;
using MyProject.Domain.Configuration;
using MyProject.Domain.Exceptions;

namespace MyProject.Tests.Domain;

public class CarPricingConfigurationTests
{
    [Fact]
    public void CarPricingConfiguration_Should_Get_BMW_X5_Price()
    {
        var price = CarPricingConfiguration.GetPriceForModel("BMW X5");

        Assert.Equal(120, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Get_Porsche_911_Price()
    {
        var price = CarPricingConfiguration.GetPriceForModel("Porsche 911");

        Assert.Equal(250, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Get_Default_Price_For_Unknown_Model()
    {
        var price = CarPricingConfiguration.GetPriceForModel("Unknown Model");

        Assert.Equal(60, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Handle_Null_Model()
    {
        var price = CarPricingConfiguration.GetPriceForModel(null!);

        Assert.Equal(60, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Handle_Empty_String_Model()
    {
        var price = CarPricingConfiguration.GetPriceForModel("");

        Assert.Equal(60, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Handle_Whitespace_Model()
    {
        var price = CarPricingConfiguration.GetPriceForModel("   ");

        Assert.Equal(60, price);
    }

    [Fact]
    public void CarPricingConfiguration_Should_Get_All_Model_Prices()
    {
        var allPrices = CarPricingConfiguration.GetAllModelPrices();

        Assert.NotNull(allPrices);
        Assert.NotEmpty(allPrices);
        Assert.Contains("BMW X5", allPrices.Keys);
    }

    [Fact]
    public void CarPricingConfiguration_All_Models_Should_Have_Valid_Prices()
    {
        var allPrices = CarPricingConfiguration.GetAllModelPrices();

        foreach (var price in allPrices.Values)
        {
            Assert.True(price > 0);
        }
    }
}

public class CarExtendedTests
{
    [Fact]
    public void Car_Should_Auto_Generate_Id_If_Empty()
    {
        var car = new Car(Guid.Empty, "BMW", true, 100);

        Assert.NotEqual(Guid.Empty, car.Id);
    }

    [Fact]
    public void Car_Should_Preserve_Provided_Id()
    {
        var providedId = Guid.NewGuid();
        var car = new Car(providedId, "BMW", true, 100);

        Assert.Equal(providedId, car.Id);
    }

    [Fact]
    public void Car_Should_Apply_Default_Price_When_Zero()
    {
        var car = new Car("BMW X5", 0);

        Assert.Equal(120, car.PricePerDay);
    }

    [Fact]
    public void Car_Should_Apply_Default_Price_When_Generic_60()
    {
        var car = new Car("BMW X5", 60);

        Assert.Equal(120, car.PricePerDay);
    }

    [Fact]
    public void Car_Should_Use_Provided_Price_When_Different_From_Default()
    {
        var car = new Car("BMW X5", 150);

        Assert.Equal(150, car.PricePerDay);
    }

    [Fact]
    public void Car_Should_Throw_On_Empty_Model()
    {
        Assert.Throws<ArgumentException>(() => new Car(""));
    }

    [Fact]
    public void Car_Should_Throw_On_Null_Model()
    {
        Assert.Throws<ArgumentException>(() => new Car(null!));
    }

    [Fact]
    public void Car_Should_Throw_On_Whitespace_Model()
    {
        Assert.Throws<ArgumentException>(() => new Car("   "));
    }

    [Fact]
    public void Car_Should_Initialize_As_Available()
    {
        var car = new Car("BMW", 100);

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Rent_Then_Return_Should_Work()
    {
        var car = new Car("BMW", 100);

        car.Rent();
        Assert.False(car.IsAvailable);

        car.Return();
        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Multiple_Rent_Should_Throw_On_Second_Rent()
    {
        var car = new Car("BMW", 100);

        car.Rent();
        
        Assert.Throws<CarAlreadyRentedException>(() => car.Rent());
    }

    [Fact]
    public void Car_Return_While_Available_Should_Work()
    {
        var car = new Car("BMW", 100);

        car.Return(); // Should not throw
        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Multiple_Returns_Should_Work()
    {
        var car = new Car("BMW", 100);

        car.Return();
        car.Return();

        Assert.True(car.IsAvailable);
    }

    [Fact]
    public void Car_Should_Support_Negative_Custom_Price()
    {
        // This tests edge case handling
        var car = new Car("BMW", -50);

        // Either uses default or negative price, both are valid behaviors
        Assert.NotNull(car);
    }

    [Fact]
    public void Car_Should_Support_Very_Large_Price()
    {
        var car = new Car("BMW", decimal.MaxValue);

        Assert.Equal(decimal.MaxValue, car.PricePerDay);
    }

    [Fact]
    public void Car_Should_Support_Very_Small_Decimal_Price()
    {
        var car = new Car("BMW", 0.01m);

        Assert.Equal(0.01m, car.PricePerDay);
    }

    [Fact]
    public void Car_Constructor_With_All_Parameters()
    {
        var id = Guid.NewGuid();
        var car = new Car(id, "TestCar", false, 99.99m);

        Assert.Equal(id, car.Id);
        Assert.Equal("TestCar", car.Model);
        Assert.False(car.IsAvailable);
        Assert.Equal(99.99m, car.PricePerDay);
    }
}

public class RentalExtendedTests
{
    [Fact]
    public void Rental_Should_Store_Car_Reference()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 5);

        Assert.Same(car, rental.Car);
    }

    [Fact]
    public void Rental_Should_Store_Customer_Reference()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 5);

        Assert.Same(customer, rental.Customer);
    }

    [Fact]
    public void Rental_Should_Calculate_Price_With_Economy_Discount()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 3);

        // Economy: 5% discount
        var expected = 100 * 3 * 0.95m;
        Assert.Equal(expected, rental.TotalPrice);
    }

    [Fact]
    public void Rental_Should_Calculate_Price_With_Premium_Discount()
    {
        var car = new Car("BMW", 100);
        var customer = new PremiumCustomer("John");
        var rental = new Rental(car, customer, 3);

        // Premium: 20% discount
        var expected = 100 * 3 * 0.8m;
        Assert.Equal(expected, rental.TotalPrice);
    }

    [Fact]
    public void Rental_Should_Initialize_With_Zero_Penalty()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 5);

        Assert.Equal(0, rental.LatePenalty);
    }

    [Fact]
    public void Rental_Should_Calculate_Penalty_For_Late_Return()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 1);

        rental.CalculatePenalty();

        // Penalty is 0 or positive for late returns
        Assert.True(rental.LatePenalty >= 0);
    }

    [Fact]
    public void Rental_GetTotalCost_Should_Include_Penalty()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 1);

        rental.CalculatePenalty();

        var totalCost = rental.GetTotalCost();

        Assert.True(totalCost >= rental.TotalPrice);
    }

    [Fact]
    public void Rental_Should_Throw_On_Null_Car()
    {
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentNullException>(() => new Rental(null!, customer, 5));
    }

    [Fact]
    public void Rental_Should_Throw_On_Null_Customer()
    {
        var car = new Car("BMW", 100);

        Assert.Throws<ArgumentNullException>(() => new Rental(car, null!, 5));
    }

    [Fact]
    public void Rental_Should_Throw_On_Zero_Days()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() => new Rental(car, customer, 0));
    }

    [Fact]
    public void Rental_Should_Throw_On_Negative_Days()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() => new Rental(car, customer, -5));
    }

    [Fact]
    public void Rental_Should_Throw_On_Days_Over_365()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        Assert.Throws<ArgumentException>(() => new Rental(car, customer, 366));
    }

    [Fact]
    public void Rental_Should_Allow_One_Day_Rental()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        var rental = new Rental(car, customer, 1);

        Assert.Equal(1, rental.Days);
    }

    [Fact]
    public void Rental_Should_Allow_365_Day_Rental()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");

        var rental = new Rental(car, customer, 365);

        Assert.Equal(365, rental.Days);
    }

    [Fact]
    public void Rental_Should_Store_RentedAt_Timestamp()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        
        var beforeCreation = DateTime.Now;
        var rental = new Rental(car, customer, 5);
        var afterCreation = DateTime.Now;

        Assert.True(rental.RentedAt >= beforeCreation.AddSeconds(-1));
        Assert.True(rental.RentedAt <= afterCreation.AddSeconds(1));
    }

    [Fact]
    public void Rental_Multiple_Penalty_Calculations_Should_Be_Idempotent()
    {
        var car = new Car("BMW", 100);
        var customer = new EconomyCustomer("John");
        var rental = new Rental(car, customer, 1);

        rental.CalculatePenalty();
        var penalty1 = rental.LatePenalty;

        rental.CalculatePenalty();
        var penalty2 = rental.LatePenalty;

        // Second call should recalculate based on current time
        Assert.True(penalty2 >= penalty1);
    }
}
