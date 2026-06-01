namespace CarRentSystem.Tests.Helpers;

public static class TestPathHelper
{
    public static string CreateTempFile()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.json");
    }
}