namespace MyProject.Application.Events;

public interface IEventListener
{
    void Handle(string message);
}