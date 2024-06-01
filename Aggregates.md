# PlayerAggregate

## Player

### Properties

- ID
- Name
- Credits
- Experience
- Rank
- CommandCenter
- Operation

### Methods

- CreatePlayer()
- EarnCredits()
- SpendCredits()
- GainExperience()

# CommandCenterAggregate

## CommandCenter

### Properties

- ID
- Player
- Planet
- Mission
- Difficulty
- HighestDifficultyAvailable

### Methods

- OpenMap()
- ChoosePlanet()
- ChooseDifficulty()
- ChooseMission()
- PrepareMission()
- StartMission()
- JoinMission()
- RewardPlayer()
- AbandonMission()

## Planet

### Properties

- Name
- Missions

### Methods

- GetMissions()

## Mission

### Properties

- Planet
- Squad
- Initiator
- Status
- Type
- Objectives

### Methods

- AddToSquad()
- RemoveFromSquad()
- Create()
- Start()
- Complete()
- Fail()
- UpdateStatus()
