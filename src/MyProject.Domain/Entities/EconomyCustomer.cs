namespace MyProject.Domain.Entities;

public class EconomyCustomer : Customer
{
    public EconomyCustomer(string name) : base(name) { }

    public override decimal GetDiscount() => 0.05m;
}