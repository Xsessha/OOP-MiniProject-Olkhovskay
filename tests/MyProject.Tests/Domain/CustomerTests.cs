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
}

