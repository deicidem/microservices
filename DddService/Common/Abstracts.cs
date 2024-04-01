namespace DddService.Common;


public abstract class Entity<T> : IEntity<T>
{
    public T Id { get; set; }
    public bool IsDeleted { get; set; }
}

// Aggregates
public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    
}