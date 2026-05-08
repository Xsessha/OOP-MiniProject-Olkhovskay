namespace MyProject.Application.Events;

public class EventManager
{
    private readonly List<IEventListener> _listeners = new();

    public void Subscribe(IEventListener listener)
    {
        _listeners.Add(listener);
    }

    public void Notify(string message)
    {
        foreach (var listener in _listeners)
        {
            listener.Handle(message);
        }
    }
}