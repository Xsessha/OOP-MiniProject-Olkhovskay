using Xunit;
using MyProject.Domain.Entities;
using MyProject.Domain.Exceptions;
using MyProject.Infrastructure.Persistence;
using MyProject.Application.Events;
using System.Collections.Generic;

namespace MyProject.Tests.Integration;

public class PersistenceErrorHandlingTests
{
    [Fact]
    public void JsonDataStore_Should_Return_Fail_On_Invalid_Path_For_Save()
    {
        var cars = new List<Car> { new Car("BMW") };
        var invalidPath = "/invalid/path/that/does/not/exist/cars.json";

        var result = JsonDataStore<Car>.Save(invalidPath, cars);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Failed to save file", result.ErrorMessage);
    }

    [Fact]
    public void JsonDataStore_Load_Should_Handle_Corrupted_Json_Gracefully()
    {
        var tempFile = Path.GetTempFileName();

        File.WriteAllText(tempFile, "{ invalid json content }}}");

        var result = JsonDataStore<Car>.LoadResult(tempFile);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Corrupted JSON", result.ErrorMessage);

        File.Delete(tempFile);
    }

    [Fact]
    public void JsonDataStore_Load_Should_Return_Empty_List_When_File_Missing()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        var result = JsonDataStore<Car>.LoadResult(missingPath);

        Assert.True(result.Success);
        Assert.Empty(result.Value ?? new List<Car>());
    }

    [Fact]
    public void JsonDataStore_Should_Retry_On_Io_Error_Multiple_Times()
    {
        var tempFile = Path.GetTempFileName();
        var cars = new List<Car> { new Car("BMW") };

        try
        {
            var result = JsonDataStore<Car>.Save(tempFile, cars);
            Assert.True(result.Success);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ApplicationEventBus_Should_Log_Persistence_Errors()
    {
        var loggedMessages = new List<string>();
        var testListener = new TestEventListener(loggedMessages);

        ApplicationEventBus.Subscribe(testListener);

        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "corrupted data");

        var result = JsonDataStore<Car>.LoadResult(tempFile);

        Assert.False(result.Success);
        Assert.NotEmpty(loggedMessages);
        Assert.True(loggedMessages.Any(m => m.Contains("Corrupted JSON")), 
            "Expected error message to be logged");

        File.Delete(tempFile);
    }

    private class TestEventListener : IEventListener
    {
        private readonly List<string> _messages;

        public TestEventListener(List<string> messages)
        {
            _messages = messages;
        }

        public void Handle(string message)
        {
            _messages.Add(message);
        }
    }
}
