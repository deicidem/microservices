CREATE TABLE "Players" (
    "Id" UUID PRIMARY KEY,
    "Nickname" VARCHAR(100),
    "Rank" VARCHAR(50),
    "Credits" INTEGER,
    "Experience" INTEGER,
    "IsDeleted" BOOLEAN
);
CREATE TABLE "Planets" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100),
    "Progress" INTEGER,
    "Status" INTEGER,
    "IsDeleted" BOOLEAN
);
CREATE TABLE "MissionTypes" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100),
    "Description" VARCHAR(200),
    "Goals" VARCHAR(200),
    "IsDeleted" BOOLEAN
);
CREATE TABLE "Missions" (
    "Id" UUID PRIMARY KEY,
    "Status" INTEGER,
    "TypeId" UUID REFERENCES "MissionTypes"("Id") NOT NULL,
    "InitiatorId" UUID REFERENCES "Players"("Id") NOT NULL,
    "PlanetId" UUID REFERENCES "Planets"("Id") NOT NULL,
    "Difficulty" INTEGER,
    "Squad" VARCHAR(200),
    "Reinforcements" INTEGER,
    "IsDeleted" BOOLEAN
);
CREATE TABLE "Objectives" (
    "Id" UUID PRIMARY KEY,
    "Goal" VARCHAR(100),
    "IsCompleted" BOOLEAN,
    "MissionId" UUID REFERENCES "Missions"("Id") NOT NULL,
    "IsDeleted" BOOLEAN
);
CREATE TABLE "CommandCenters" (
    "Id" UUID PRIMARY KEY,
    "PlayerId" UUID REFERENCES "Players"("Id") NOT NULL,
    "PlanetId" UUID REFERENCES "Planets"("Id") NULL,
    "HighestDifficultyAvailable" INTEGER,
    "MissionId" UUID REFERENCES "Missions"("Id") NULL,
    "IsDeleted" BOOLEAN
);

-- Add CommandCenterId to Players table
ALTER TABLE "Players" ADD COLUMN "CommandCenterId" UUID REFERENCES "CommandCenters"("Id") NULL;
-- CREATE TABLE weapon (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     type VARCHAR(50),
--     damage INTEGER,
--     fire_rate INTEGER,
--     ammunition INTEGER
-- );

-- CREATE TABLE soldier (
--     id SERIAL PRIMARY KEY,
--     health INTEGER,
--     owner_id INTEGER,
--     weapon_id INTEGER,
--     FOREIGN KEY (owner_id) REFERENCES player("Id"),
--     FOREIGN KEY (weapon_id) REFERENCES weapon("Id")
-- );

-- CREATE TABLE enemy_factions (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     description VARCHAR(500)
-- );

-- CREATE TABLE enemy (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     health INTEGER,
--     damage INTEGER,
--     faction_id INTEGER,
--     FOREIGN KEY (faction_id) REFERENCES enemy_factions("Id")
-- );

-- CREATE TABLE sector (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     liberation_status VARCHAR(50),
--     faction_id INTEGER,
--     FOREIGN KEY (faction_id) REFERENCES enemy_factions("Id")
-- );

-- CREATE TABLE planet (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     liberation_progress INTEGER,
--     current_players INTEGER,
--     sector_id INTEGER,
--     FOREIGN KEY (sector_id) REFERENCES sector("Id")
-- );

-- CREATE TABLE difficulty (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     description VARCHAR(100)
-- );

-- CREATE TABLE mission (
--     id SERIAL PRIMARY KEY,
--     name VARCHAR(100),
--     mission_type VARCHAR(50),
--     difficulty_id INTEGER,
--     initiator_id INTEGER,
--     planet_id INTEGER,
--     FOREIGN KEY (initiator_id) REFERENCES player("Id"),
--     FOREIGN KEY (planet_id) REFERENCES planet("Id"),
--     FOREIGN KEY (difficulty_id) REFERENCES difficulty("Id")
-- );

-- COPY player FROM "/etc/postgresql/init-script/player.csv" DELIMITER "," CSV HEADER;
-- COPY weapon FROM "/etc/postgresql/init-script/weapon.csv" DELIMITER "," CSV HEADER;
-- COPY soldier FROM "/etc/postgresql/init-script/soldier.csv" DELIMITER "," CSV HEADER;
-- COPY enemy_factions FROM "/etc/postgresql/init-script/enemy_factions.csv" DELIMITER "," CSV HEADER;
-- COPY enemy FROM "/etc/postgresql/init-script/enemy.csv" DELIMITER "," CSV HEADER;
-- COPY sector FROM "/etc/postgresql/init-script/sector.csv" DELIMITER "," CSV HEADER;
-- COPY planet FROM "/etc/postgresql/init-script/planet.csv" DELIMITER "," CSV HEADER;
-- COPY difficulty FROM "/etc/postgresql/init-script/difficulty.csv" DELIMITER "," CSV HEADER;
-- COPY mission FROM "/etc/postgresql/init-script/mission.csv" DELIMITER "," CSV HEADER;
