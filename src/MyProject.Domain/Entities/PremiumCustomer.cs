namespace MyProject.Domain.Entities;

public class PremiumCustomer : Customer
{
    public PremiumCustomer(string name) : base(name) { }

    public override decimal GetDiscount() => 0.2m;
}