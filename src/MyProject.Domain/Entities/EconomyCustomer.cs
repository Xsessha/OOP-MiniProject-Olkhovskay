namespace MyProject.Domain.Entities;

public class EconomyCustomer : Customer
{
    public EconomyCustomer(string name) : base(name) { }

    public override string CustomerType => "Economy";
    public override decimal GetDiscount() => 0.05m;
}