using Xunit;
using CarRentSystem.Tests.Helpers;

namespace CarRentSystem.Tests.Helpers;

public class HelperTests
{
    [Fact]
    public void Helper_Should_Create_Json_Path()
    {
        var path = TestPathHelper.CreateTempFile();

        Assert.Contains(".json", path);
    }

    [Fact]
    public void Helper_Should_Create_Unique_Paths()
    {
        var p1 = TestPathHelper.CreateTempFile();
        var p2 = TestPathHelper.CreateTempFile();

        Assert.NotEqual(p1, p2);
    }
}