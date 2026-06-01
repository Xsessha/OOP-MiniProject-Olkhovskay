namespace CarRentSystem.Application.Events;

public static class ApplicationEventBus
{
    private static readonly EventManager _eventManager = new();

    public static void Subscribe(IEventListener listener)
    {
        _eventManager.Subscribe(listener);
    }

    public static void Notify(string message)
    {
        _eventManager.Notify(message);
    }
}
