namespace DddService.Common;

// For handling optimistic concurrency
public interface IVersion
{
    long Version { get; set; }
}


public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
    void AddDomainEvent(IDomainEvent domainEvent);
    IReadOnlyList<IDomainEvent> GetDomainEvents();
    IEvent[] ClearDomainEvents();
}

// Entity

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

public interface IEntity
{
    public bool IsDeleted { get; set; }
}