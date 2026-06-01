namespace CarRentSystem.Application.Events;

public class ConsoleLogger : IEventListener
{
    public void Handle(string message)
    {
        Console.WriteLine($" EVENT: {message}");
    }
}