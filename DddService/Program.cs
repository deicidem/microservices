using System.Reflection;
using DddService.Aggregates;
using DddService.Aggregates.CommandCenterNamespace;
using DddService.Aggregates.MissionNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;
using DddService.Database;
using DddService.Dto;
using DddService.EventBus;
using DddService.Features.CommandCenterFeature;
using DddService.Features.MissionFeature;
using DddService.Features.PlayerFeature;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HelldiversDbContext>(options =>
{
    options.UseNpgsql("Host=postgres_master;Port=5432;Database=helldivers;Username=postgres;Password=postgres",
        b => b.MigrationsAssembly("DddService"));
}, ServiceLifetime.Transient);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddHostedService<RewardGrantedConsumerService>();
var app = builder.Build();
builder.Services.AddLogging(e => e.AddConsole());
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SeedData(app);
var mediator = app.Services.GetRequiredService<IMediator>();
// var RewardGrantedConsumerService = app.Services.GetRequiredService<IHostedService>();
// await RewardGrantedConsumerService.StartAsync(default);
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


app.MapPost("api/players", async (PlayerInputModel model, IMediator mediator) =>
{
    var command = new CreatePlayerCommand(model.Nickname);
    var response = await mediator.Send(command);

    return Results.Created($"api/players/{response.Id}", response);
});

app.MapGet("api/players", async (IMediator mediator) =>
{
    return await mediator.Send(new GetAllPlayersQuery());
});

app.MapGet("api/command-center", async (string playerId, IMediator mediator) =>
{
    return await mediator.Send(new GetCommandCenterByIdQuery(playerId));
});

app.MapGet("api/planets", async (IMediator mediator) =>
{
    return await mediator.Send(new GetAllPlanetsQuery());
});

app.MapGet("api/mission-types", async (IMediator mediator) =>
{
    return await mediator.Send(new GetAllMissionTypesQuery());
});

app.MapPost("api/command-center/{commandCenterId}/mission/initiate", async (string commandCenterId, MissionInitiateModel missionInitiate, IMediator mediator) =>
{
    var command = new CreateMissionCommand(commandCenterId, missionInitiate.MissionTypeId, missionInitiate.PlanetId, missionInitiate.Difficulty);
    var response = await mediator.Send(command);
    return Results.Created($"api/missions/{response.Id}", response);
});

app.MapPost("api/command-center/{commandCenterId}/mission/search", async (string commandCenterId, IMediator mediator) =>
{
    var command = new SearchForMissionCommand(commandCenterId);
    return await mediator.Send(command);
});

app.MapPost("api/command-center/{commandCenterId}/mission/start", async (string commandCenterId, IMediator mediator) =>
{
    var command = new StartMissionCommand(commandCenterId);
    return await mediator.Send(command);
});

app.MapPost("api/command-center/{commandCenterId}/mission/complete-objective", async (string commandCenterId, IMediator mediator) =>
{
    var command = new CompleteObjectiveCommand(commandCenterId);
    await mediator.Send(command);
    return Results.Ok();
});

app.MapPost("api/command-center/{commandCenterId}/mission/abandon", async (string commandCenterId, IMediator mediator) =>
{
    var command = new AbandonMissionCommand(commandCenterId);
    await mediator.Send(command);
    return Results.Ok();
});

app.Run();
// var consumer = app.Services.GetService<RewardGrantedConsumerService>();
// Task.Run(async () =>
// {
//     await consumer.StartAsync(CancellationToken.None);
// });