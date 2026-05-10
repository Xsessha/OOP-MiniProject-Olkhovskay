namespace MyProject.Tests.Application;

public class RentalDtoTests
{
    [Fact]
    public void RentalDto_Should_Store_CarModel_And_CustomerName()
    {
        var dto = new RentalDto
        {
            CarModel = "BMW X5",
            CustomerName = "Alice"
        };

        Assert.Equal("BMW X5", dto.CarModel);
        Assert.Equal("Alice", dto.CustomerName);
    }
}
