namespace DddService.Common;

// For handling optimistic concurrency
public interface IVersion
{
    long Version { get; set; }
}

// Aggregates Interfaces

public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
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