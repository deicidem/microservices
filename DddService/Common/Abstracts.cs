namespace DddService.Common;


public abstract class Entity : IEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}

// Aggregates
public abstract class Aggregate : Entity, IAggregate
{

}