namespace MyProject.Application.Events;

public class RentalEventListener : IEventListener
{
    public void Handle(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}