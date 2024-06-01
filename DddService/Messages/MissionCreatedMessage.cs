using DddService.Aggregates.MissionNamespace;

namespace Messages;

public record MissionCreatedMessage(Guid id, Guid playerId, string typeName, string planetName, Difficulty difficulty, string timestamp);
