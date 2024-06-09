using DddService.Aggregates.MissionNamespace;

namespace Messages;

public record RewardGrantedMessage(
    Guid PlayerId,
    int Experience,
    int Credits,
    Difficulty Difficulty,
    string Timestamp);
