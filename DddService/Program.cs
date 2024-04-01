using DddService.Aggregates;
using DddService.Common;
using DddService.Dto;
using Microsoft.EntityFrameworkCore;


// Requests
// /space-images CRUD
// /space-images (GET)
// /space-images (POST) - только ссылка на файл (загрузка файла через другой микросервис)
// /space-images/{spaceImageId} (GET)
// /space-images/{spaceImageId} (PUT)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AirportDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=airport;Username=postgres;Password=postgres",
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
        using (var db = scope.ServiceProvider.GetRequiredService<AirportDbContext>())
        {
            db.Database.EnsureCreated();
            if (!db.Airports.Any())
            {
                db.Airports.Add(Airport.Create(AirportId.Of(Guid.NewGuid()), Name.Of("Minsk"), Address.Of("Minsk"), Code.Of("MCQ")));
                db.SaveChanges();
            }
        }
    }
}


app.MapPost("api/airports", async (AirportInputModel model, AirportDbContext db) =>
{
    var airport = await db.Airports.SingleOrDefaultAsync(x => x.Code.Value == model.Code);

    if (airport is not null)
    {
        //throw new AirportAlreadyExistException();
        return Results.BadRequest("Airport already exist!");
    }

    var airportEntity = db.Airports.Add(Airport.Create(AirportId.Of(Guid.NewGuid()), Name.Of(model.Name), Address.Of(model.Address), Code.Of(model.Code))).Entity;
    await db.SaveChangesAsync();
    return Results.Created($"api/airports/{airportEntity.Id}", airportEntity);
});


app.MapGet("api/airports", async (AirportDbContext db) => { return await db.Airports.Select(a => new AirportDto(a.Id.Value.ToString(), a.Name.Value, a.Address.Value, a.Code.Value)).ToListAsync(); });

app.Run();

public class AirportDbContext : DbContext
{
    public DbSet<Airport> Airports { get; set; }

    public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
    {
    }

    public AirportDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Airport>().HasKey(p => p.Id);

        modelBuilder.Entity<Airport>().ToTable(nameof(Airport));

        modelBuilder.Entity<Airport>().HasKey(r => r.Id);
        modelBuilder.Entity<Airport>().Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(airportId => airportId.Value, dbId => AirportId.Of(dbId));

        modelBuilder.Entity<Airport>().OwnsOne(
            x => x.Name,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Airport.Name))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Airport>().OwnsOne(
            x => x.Address,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Airport.Address))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        modelBuilder.Entity<Airport>().OwnsOne(
            x => x.Code,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Airport.Code))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );
    }
}
