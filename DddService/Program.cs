using System.Reflection;
using DddService.Aggregates;
using DddService.Aggregates.CommandCenterNamespace;
using DddService.Aggregates.MissionNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;
using DddService.Dto;
using DddService.EventBus;
using DddService.Features.CreatingMission;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HelldiversDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5433;Database=helldivers;Username=postgres;Password=postgres",
        b => b.MigrationsAssembly("DddService"));
});
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
var app = builder.Build();
builder.Services.AddLogging(e => e.AddConsole());
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
                    Guid.NewGuid(),
                    PlanetName.Of("Earth"),
                    LiberationStatus.Liberated
                ));
                db.SaveChanges();
            }

            if (!db.MissionTypes.Any())
            {
                db.MissionTypes.Add(MissionType.Create(
                    Guid.NewGuid(),
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
        Guid.NewGuid(),
        Nickname.Of(model.Nickname)
    );
    var PlayerEntity = db.Players.Add(newPlayer).Entity;
    await db.SaveChangesAsync();

    var newCommandCenter = CommandCenter.Create(
        Guid.NewGuid(),
        newPlayer
    );
    newCommandCenter = db.CommandCenters.Add(newCommandCenter).Entity;
    await db.SaveChangesAsync();


    PlayerEntity.ConnectToCommandCenter(newCommandCenter);

    await db.SaveChangesAsync();

    return Results.Created($"api/Players/{PlayerEntity.Id}", new PlayerDto(
        PlayerEntity.Id.ToString(),
        PlayerEntity.Nickname.Value,
        PlayerEntity.Credits.Value,
        PlayerEntity.Experience.Value,
        PlayerEntity.Rank.Value
    ));
});

app.MapGet("api/players", async (HelldiversDbContext db) =>
{
    return await db.Players.Select(a => new PlayerDto(
        a.Id.ToString(),
        a.Nickname.Value,
        a.Credits.Value,
        a.Experience.Value,
        a.Rank.Value
    ))
    .ToListAsync();
});

app.MapGet("api/command-center", async (string playerId, HelldiversDbContext db) =>
{
    var commandCenter = await db.CommandCenters.FirstOrDefaultAsync(p => p.PlayerId == Guid.Parse(playerId));
    if (commandCenter is null)
    {
        return Results.NotFound();
    }

    return Results.Json(new CommandCenterDto(
        commandCenter.Id.ToString(),
        commandCenter.PlayerId.ToString(),
        commandCenter.MissionId?.ToString(),
        commandCenter.HighestDifficultyAvailable.ToString()
    ));

});

app.MapGet("api/planets", async (HelldiversDbContext db) =>
{
    return await db.Planets.Select(p => new PlanetDto(
        p.Id.ToString(),
        p.Name.Value,
        p.Status.GetDisplayName(),
        p.Progress.Value
    )).ToListAsync();
});

app.MapGet("api/mission-types", async (HelldiversDbContext db) =>
{
    return await db.MissionTypes.Select(p => new MissionTypeDto(
        p.Id.ToString(),
        p.Name.Value,
        p.Description.Value,
        p.Goals.GetGoals()
    )).ToListAsync();
});

app.MapPost("api/command-center/{commandCenterId}/mission/initiate", async (string commandCenterId, MissionInitiateModel missionInitiate, HelldiversDbContext db, IMediator mediator) =>
{
    var command = new CreateMissionCommand(commandCenterId, missionInitiate.MissionTypeId, missionInitiate.PlanetId, missionInitiate.Difficulty);
    var response = await mediator.Send(command);
    return Results.Ok();
});

app.MapPost("api/command-center/{commandCenterId}/mission/search", async (string commandCenterId, HelldiversDbContext db) =>
{
    var commandCenter = await db.CommandCenters.FirstOrDefaultAsync(p => p.Id == Guid.Parse(commandCenterId));
    if (commandCenter is null)
    {
        return Results.NotFound();
    }

    var missions = await db.Missions.ToListAsync();
    var mission = commandCenter.SearchForMission(missions);

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("api/command-center/{commandCenterId}/mission/start", async (string commandCenterId, HelldiversDbContext db) =>
{
    var commandCenter = await db.CommandCenters.FirstOrDefaultAsync(p => p.Id == Guid.Parse(commandCenterId));
    if (commandCenter is null)
    {
        return Results.NotFound();
    }

    commandCenter.StartMission();

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapPost("api/command-center/{commandCenterId}/mission/abandon", async (string commandCenterId, HelldiversDbContext db) =>
{
    var commandCenter = await db.CommandCenters.Include(c => c.Player).FirstOrDefaultAsync(p => p.Id == Guid.Parse(commandCenterId));
    if (commandCenter is null)
    {
        return Results.NotFound();
    }

    commandCenter.AbandonMission();

    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();

public class HelldiversDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Planet> Planets { get; set; }
    public DbSet<MissionType> MissionTypes { get; set; }
    public DbSet<Objective> Objectives { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<CommandCenter> CommandCenters { get; set; }
    private IMediator _mediator;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }
    }
    public HelldiversDbContext(DbContextOptions<HelldiversDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
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
        modelBuilder.Entity<Player>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<Player>().ComplexProperty(p => p.Nickname, p =>
        {
            p.Property(p => p.Value).HasColumnName(nameof(Player.Nickname))
                    .HasMaxLength(50)
                    .IsRequired();
        });

        modelBuilder.Entity<Player>().ComplexProperty(
            x => x.Credits,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Credits))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Player>().ComplexProperty(
            x => x.Experience,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Player.Experience))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Player>().ComplexProperty(
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
        // Objectives
        modelBuilder.Entity<Objective>().ToTable("Objectives");

        modelBuilder.Entity<Objective>().HasKey(r => r.Id);
        modelBuilder.Entity<Objective>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<Objective>().ComplexProperty(
            x => x.Goal,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Objective.Goal))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );
        modelBuilder.Entity<Objective>().ComplexProperty(
            x => x.IsCompleted,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Objective.IsCompleted))
                    .IsRequired();
            }
        );
        modelBuilder.Entity<Objective>()
        .HasOne(o => o.Mission)
        .WithMany(m => m.Objectives) // Миссия может иметь много целей
        .HasForeignKey(o => o.MissionId); // Внешний ключ
        // MissionTypes
        modelBuilder.Entity<MissionType>().ToTable("MissionTypes");
        modelBuilder.Entity<MissionType>().HasKey(r => r.Id);
        modelBuilder.Entity<MissionType>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<MissionType>().ComplexProperty(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(MissionType.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<MissionType>().ComplexProperty(
            x => x.Description,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(MissionType.Description))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<MissionType>().ComplexProperty(
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
        modelBuilder.Entity<Planet>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<Planet>().ComplexProperty(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Planet.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Planet>().ComplexProperty(
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
        modelBuilder.Entity<Mission>().Property(r => r.Id).ValueGeneratedNever();

        modelBuilder.Entity<Mission>().ComplexProperty(
            x => x.Reinforcements,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Mission.Reinforcements))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Mission>().ComplexProperty(
            x => x.Squad,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Mission.Squad))
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Mission>().Property(
            x => x.Difficulty
        );

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



        // CommandCenters
        modelBuilder.Entity<CommandCenter>().ToTable("CommandCenters");
        modelBuilder.Entity<CommandCenter>().HasKey(r => r.Id);
        modelBuilder.Entity<CommandCenter>().Property(r => r.Id).ValueGeneratedNever();


        modelBuilder.Entity<CommandCenter>().Property(
            x => x.HighestDifficultyAvailable
        );


        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Player)
            .WithOne()
            .HasForeignKey<CommandCenter>(m => m.PlayerId);


        modelBuilder.Entity<CommandCenter>()
            .HasOne(m => m.Mission)
            .WithOne()
            .HasForeignKey<CommandCenter>(m => m.MissionId);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch Domain Events collection.
        await DispatchEvents(cancellationToken);

        return result;
    }
    private async Task DispatchEvents(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.GetDomainEvents() != null && x.Entity.GetDomainEvents().Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.GetDomainEvents())
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }
}
