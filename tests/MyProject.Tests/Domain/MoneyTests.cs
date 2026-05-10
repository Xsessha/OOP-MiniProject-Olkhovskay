using MyProject.Domain.ValueObjects;

namespace MyProject.Tests.Domain;

public class MoneyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(99.99)]
    [InlineData(1000000)]
    public void Constructor_Should_Store_NonNegative_Amount(decimal amount)
    {
        var money = new Money(amount);

        Assert.Equal(amount, money.Amount);
    }

    [Fact]
    public void Constructor_Should_Reject_Negative_Amount()
    {
        var exception = Assert.Throws<ArgumentException>(() => new Money(-0.01m));

        Assert.Contains("negative", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Addition_Should_Return_New_Money_With_Summed_Amount()
    {
        var first = new Money(99.99m);
        var second = new Money(0.01m);

        var result = first + second;

        Assert.NotSame(first, result);
        Assert.Equal(100m, result.Amount);
    }

    [Fact]
    public void GreaterThan_Should_Compare_Amounts()
    {
        Assert.True(new Money(100) > new Money(50));
        Assert.False(new Money(50) > new Money(100));
        Assert.False(new Money(100) > new Money(100));
    }

    [Fact]
    public void LessThan_Should_Compare_Amounts()
    {
        Assert.True(new Money(50) < new Money(100));
        Assert.False(new Money(100) < new Money(50));
        Assert.False(new Money(100) < new Money(100));
    }
}
