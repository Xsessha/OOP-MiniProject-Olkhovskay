namespace MyProject.Domain.Entities;

public class PremiumCustomer : Customer
{
    public PremiumCustomer(string name) : base(name) { }

    public override string CustomerType => "Premium";
    public override decimal GetDiscount() => 0.20m;
}