using DddService.Aggregates;
using DddService.Aggregates.CommandCenterNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;
using DddService.Dto;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HelldiversDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5433;Database=helldivers;Username=postgres;Password=postgres",
        b => b.MigrationsAssembly("DddService"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SeedData(app);

// Seed data
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetRequiredService<HelldiversDbContext>())
        {
            db.Database.EnsureCreated();

            if (!db.Planets.Any())
            {
                db.Planets.Add(Planet.Create(
                    PlanetId.Of(Guid.NewGuid()),
                    PlanetName.Of("Earth"),
                    LiberationStatus.Liberated
                ));
                db.SaveChanges();
            }

            // if (!db.Difficulties.Any())
            // {
            //     db.Difficulties.Add(Difficulty.Create(
            //         DifficultyId.Of(Guid.NewGuid()),
            //         DifficultyName.Of("Easy"),
            //         DifficultyLevel.Of(1)
            //     ));
            //     db.SaveChanges();
            // }

            if (!db.MissionTypes.Any())
            {
                db.MissionTypes.Add(MissionType.Create(
                    MissionTypeId.Of(Guid.NewGuid()),
                    Name.Of("Mission Type 1"),
                    Description.Of("Description 1"),
                    Goals.Of([Goal.Of("Goal 1"), Goal.Of("Goal 2")])
                ));
                db.SaveChanges();
            }
        }
    }
}


app.MapPost("api/players", async (PlayerInputModel model, HelldiversDbContext db) =>
{
    var Player = await db.Players.SingleOrDefaultAsync(x => x.Nickname.Value == model.Nickname);

    if (Player is not null)
    {
        return Results.BadRequest("Player with this nickname already exist!");
    }
    var newPlayer = Player.Create(
        PlayerId.Of(Guid.NewGuid()),
        Nickname.Of(model.Nickname)
    );
    var PlayerEntity = db.Players.Add(newPlayer).Entity;
    await db.SaveChangesAsync();


    var baseDifficulty = db.Difficulties.FirstOrDefault(x => x.Level.Value == 1);
    if (baseDifficulty is null)
    {
        baseDifficulty = Difficulty.Create(
            DifficultyId.Of(Guid.NewGuid()),
            DifficultyName.Of("Easy"),
            DifficultyLevel.Of(1)
        );
        baseDifficulty = db.Difficulties.Add(baseDifficulty).Entity;
        await db.SaveChangesAsync();
    }

    var newCommandCenter = CommandCenter.Create(
        CommandCenterId.Of(Guid.NewGuid()),
        newPlayer,
        baseDifficulty
    );
    newCommandCenter = db.CommandCenters.Add(newCommandCenter).Entity;
    await db.SaveChangesAsync();


    PlayerEntity.ConnectToCommandCenter(newCommandCenter);

    await db.SaveChangesAsync();

    return Results.Created($"api/Players/{PlayerEntity.Id}", new PlayerDto(
        PlayerEntity.Id.Value.ToString(),
        PlayerEntity.Nickname.Value,
        PlayerEntity.Credits.Value,
        PlayerEntity.Experience.Value,
        PlayerEntity.Rank.Value
    ));
});

app.MapGet("api/players", async (HelldiversDbContext db) =>
{
    return await db.Players.Select(a => new PlayerDto(
        a.Id.Value.ToString(),
        a.Nickname.Value,
        a.Credits.Value,
        a.Experience.Value,
        a.Rank.Value
    ))
    .ToListAsync();
});
app.MapGet("api/command-center/map", async (string playerNickname, HelldiversDbContext db) =>
{
    var players = await db.Players.ToListAsync();
    var player = players.FirstOrDefault();
    var dtos = players.Select(a => new PlayerDto(
        a.Id.Value.ToString(),
        a.Nickname.Value,
        a.Credits.Value,
        a.Experience.Value,
        a.Rank.Value
    ));
    return dtos;
});


app.Run();

public class HelldiversDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Difficulty> Difficulties { get; set; }
    public DbSet<Planet> Planets { get; set; }
    public DbSet<MissionType> MissionTypes { get; set; }
    public DbSet<Objective> Objectives { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<CommandCenter> CommandCenters { get; set; }


    public HelldiversDbContext(DbContextOptions<HelldiversDbContext> options) : base(options)
    {
    }

    public HelldiversDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Players
        modelBuilder.Entity<Player>().ToTable("Players");
        modelBuilder.Entity<Player>().HasKey(r => r.Id);
        modelBuilder.Entity<Player>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(PlayerId => PlayerId.Value, dbId => PlayerId.Of(dbId));

        modelBuilder.Entity<Player>().OwnsOne(
            x => x.Nickname,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Nickname))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Player>().OwnsOne(
            x => x.Credits,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Credits))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Player>().OwnsOne(
            x => x.Experience,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Experience))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Player>().OwnsOne(
            x => x.Rank,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Rank))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );
        modelBuilder.Entity<Player>()
            .HasOne(m => m.CommandCenter)
            .WithOne()
            .HasForeignKey<Player>(m => m.CommandCenterId);

        // Difficulties
        modelBuilder.Entity<Difficulty>().ToTable("Difficulties");
        modelBuilder.Entity<Difficulty>().HasKey(r => r.Id);
        modelBuilder.Entity<Difficulty>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(DifficultyId => DifficultyId.Value, dbId => DifficultyId.Of(dbId));

        modelBuilder.Entity<Difficulty>().OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Difficulty.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );
        modelBuilder.Entity<Difficulty>().OwnsOne(
            x => x.Level,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Difficulty.Level))
                    .IsRequired();
            }
        );

        // Objectives
        modelBuilder.Entity<Objective>().ToTable("Objectives");

        modelBuilder.Entity<Objective>().HasKey(r => r.Id);
        modelBuilder.Entity<Objective>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(ObjectiveId => ObjectiveId.Value, dbId => ObjectiveId.Of(dbId));
        modelBuilder.Entity<Objective>().OwnsOne(
            x => x.Goal,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Objective.Goal))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );
        modelBuilder.Entity<Objective>().OwnsOne(
            x => x.IsCompleted,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Objective.IsCompleted))
                    .IsRequired();
            }
        );

        // MissionTypes
        modelBuilder.Entity<MissionType>().ToTable("MissionTypes");
        modelBuilder.Entity<MissionType>().HasKey(r => r.Id);
        modelBuilder.Entity<MissionType>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(MissionTypeId => MissionTypeId.Value, dbId => MissionTypeId.Of(dbId));

        modelBuilder.Entity<MissionType>().OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(MissionType.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<MissionType>().OwnsOne(
            x => x.Description,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(MissionType.Description))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<MissionType>().OwnsOne(
            x => x.Goals,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(MissionType.Goals))
                    .IsRequired();
            }
        );

        // Planets
        modelBuilder.Entity<Planet>().ToTable("Planets");
        modelBuilder.Entity<Planet>().HasKey(r => r.Id);
        modelBuilder.Entity<Planet>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(PlanetId => PlanetId.Value, dbId => PlanetId.Of(dbId));

        modelBuilder.Entity<Planet>().OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Planet.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Planet>().OwnsOne(
            x => x.Progress,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Planet.Progress))
                    .IsRequired();
            }
        );

        // Missions
        modelBuilder.Entity<Mission>().ToTable("Missions");
        modelBuilder.Entity<Mission>().HasKey(r => r.Id);
        modelBuilder.Entity<Mission>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(MissionId => MissionId.Value, dbId => MissionId.Of(dbId));

        modelBuilder.Entity<Mission>().OwnsOne(
            x => x.Reinforcements,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Mission.Reinforcements))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Mission>().OwnsOne(
            x => x.Squad,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Mission.Squad))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Mission>()
            .HasOne(m => m.Difficulty)
            .WithMany()
            .HasForeignKey(m => m.DifficultyId);

        modelBuilder.Entity<Mission>()
            .HasOne(m => m.Initiator)
            .WithMany()
            .HasForeignKey(m => m.InitiatorId);

        modelBuilder.Entity<Mission>()
            .HasOne(m => m.Planet)
            .WithMany()
            .HasForeignKey(m => m.PlanetId);

        modelBuilder.Entity<Mission>()
            .HasOne(m => m.Type)
            .WithMany()
            .HasForeignKey(m => m.TypeId);

        modelBuilder.Entity<Mission>()
            .HasMany(m => m.Objectives)
            .WithOne()
            .HasForeignKey(m => m.MissionId);


        // CommandCenters
        modelBuilder.Entity<CommandCenter>().ToTable("CommandCenters");
        modelBuilder.Entity<CommandCenter>().HasKey(r => r.Id);
        modelBuilder.Entity<CommandCenter>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(CommandCenterId => CommandCenterId.Value, dbId => CommandCenterId.Of(dbId));

        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Difficulty)
            .WithMany()
            .HasForeignKey(m => m.DifficultyId);

        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.HighestDifficultyAvailable)
            .WithMany()
            .HasForeignKey(m => m.HighestDifficultyAvailableId);

        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Player)
            .WithOne()
            .HasForeignKey<CommandCenter>(m => m.PlayerId);

        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Mission)
            .WithOne()
            .HasForeignKey<CommandCenter>(m => m.MissionId);

        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Planet)
            .WithMany()
            .HasForeignKey(m => m.PlanetId);

    }
}
