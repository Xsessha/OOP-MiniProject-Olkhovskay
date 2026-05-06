namespace MyProject.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");

        Amount = amount;
    }

    public static Money operator +(Money a, Money b)
    {
        return new Money(a.Amount + b.Amount);
    }

    public static bool operator >(Money a, Money b)
    {
        return a.Amount > b.Amount;
    }

    public static bool operator <(Money a, Money b)
    {
        return a.Amount < b.Amount;
    }
}