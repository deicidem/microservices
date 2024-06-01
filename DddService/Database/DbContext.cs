using DddService.Aggregates.CommandCenterNamespace;
using DddService.Aggregates.MissionNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Database;
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
