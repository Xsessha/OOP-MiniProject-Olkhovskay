using CarRentSystem.Application.Events;

namespace CarRentSystem.Tests.Application.Events;

public class ConsoleLoggerTests
{
    [Theory]
    [InlineData("Test event message")]
    [InlineData("")]
    [InlineData(null)]
    public void Handle_Should_Write_Event_Message(string? message)
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsole(() => logger.Handle(message!));

        Assert.Contains("EVENT:", output);
        if (message is not null)
            Assert.Contains(message, output);
    }

    [Fact]
    public void ConsoleLogger_Should_Implement_IEventListener()
    {
        Assert.IsAssignableFrom<IEventListener>(new ConsoleLogger());
    }

    private static string CaptureConsole(Action action)
    {
        lock (ConsoleCapture.SyncRoot)
        {
            var originalOut = Console.Out;
            using var writer = new StringWriter();

            try
            {
                Console.SetOut(writer);
                action();
                return writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}

public class RentalEventListenerTests
{
    [Theory]
    [InlineData("Rental created")]
    [InlineData("")]
    [InlineData(null)]
    public void Handle_Should_Write_Log_Message(string? message)
    {
        var listener = new RentalEventListener();

        var output = CaptureConsole(() => listener.Handle(message!));

        Assert.Contains("[LOG]", output);
        if (message is not null)
            Assert.Contains(message, output);
    }

    [Fact]
    public void RentalEventListener_Should_Implement_IEventListener()
    {
        Assert.IsAssignableFrom<IEventListener>(new RentalEventListener());
    }

    private static string CaptureConsole(Action action)
    {
        lock (ConsoleCapture.SyncRoot)
        {
            var originalOut = Console.Out;
            using var writer = new StringWriter();

            try
            {
                Console.SetOut(writer);
                action();
                return writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}

internal static class ConsoleCapture
{
    public static readonly object SyncRoot = new();
}

public class ApplicationEventBusTests
{
    [Fact]
    public void Notify_Should_Pass_Message_To_Subscribed_Listeners()
    {
        var listener1 = new MockEventListener();
        var listener2 = new MockEventListener();

        ApplicationEventBus.Subscribe(listener1);
        ApplicationEventBus.Subscribe(listener2);

        ApplicationEventBus.Notify("Important event");

        Assert.True(listener1.EventReceived);
        Assert.True(listener2.EventReceived);
        Assert.Equal("Important event", listener1.LastMessage);
        Assert.Equal("Important event", listener2.LastMessage);
    }

    private class MockEventListener : IEventListener
    {
        public bool EventReceived { get; private set; }
        public string? LastMessage { get; private set; }

        public void Handle(string message)
        {
            EventReceived = true;
            LastMessage = message;
        }
    }
}
