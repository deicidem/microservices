using MediatR;

namespace DddService.Common;

public interface IEvent : INotification
{
    // Лучше - https://github.com/phatboyg/NewId
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}