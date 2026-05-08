using MyProject.Domain.Entities;
using Xunit;

public class CustomerTests
{
    [Fact]
    public void ShouldThrowIfNameEmpty()
    {
        Assert.Throws<ArgumentException>(() => new EconomyCustomer(""));
    }
    [Fact]
    public void PremiumCustomerShouldHaveHigherDiscount()
   {
    var premium = new PremiumCustomer("Alex");
    var economy = new EconomyCustomer("Bob");

    Assert.True(premium.GetDiscount() > economy.GetDiscount());
    }

    [Fact]
    public void DiscountsShouldBePositive()
    {
    var premium = new PremiumCustomer("Alex");

    Assert.True(premium.GetDiscount() > 0);
    }
    [Fact]
    public void Should_Throw_If_Name_Empty()
    {
        Assert.Throws<ArgumentException>(() => new EconomyCustomer(""));
    }

    [Fact]
    public void Should_Throw_If_Name_Null()
    {
        Assert.Throws<ArgumentException>(() => new EconomyCustomer(null!));
    }

    [Fact]
    public void Premium_Should_Have_Higher_Discount()
    {
        var premium = new PremiumCustomer("Alex");
        var economy = new EconomyCustomer("Bob");

        Assert.True(premium.GetDiscount() > economy.GetDiscount());
    }

    [Fact]
    public void Discount_Should_Not_Be_Negative()
    {
        var c1 = new EconomyCustomer("Test");
        var c2 = new PremiumCustomer("Test");

        Assert.True(c1.GetDiscount() >= 0);
        Assert.True(c2.GetDiscount() >= 0);
    }

    [Fact]
    public void Discount_Should_Be_Stable()
    {
        var customer = new PremiumCustomer("Alex");

        var d1 = customer.GetDiscount();
        var d2 = customer.GetDiscount();

        Assert.Equal(d1, d2);
    }

    [Fact]
    public void Name_Should_Be_Stored()
    {
        var customer = new EconomyCustomer("John");

        Assert.Equal("John", customer.Name);
    }
}

