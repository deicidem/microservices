using System.Text.Json;
using Consul;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPlayersService, PlayersServiceClient>();

builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
{
    consulConfig.Address = new Uri("http://consul:8500");
}));

builder.Services.Configure<ServiceDiscoveryConfig>(builder.Configuration.GetSection("ServiceDiscoveryConfig"));

var app = builder.Build();

app.UseConsul();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/first-player", async (HttpContext httpContext, IPlayersService playersService) =>
{
    var players = playersService.GetPlayers();

    var firstPlayer = players.First();

    httpContext.Response.ContentType = "application/json";
    await JsonSerializer.SerializeAsync(httpContext.Response.Body, firstPlayer);
});

app.Run();

public interface IPlayersService
{
    List<Player> GetPlayers();
}


public class PlayersServiceClient : IPlayersService
{
    private readonly HttpClient _httpClient;
    private readonly IConsulClient _consulClient;

    public PlayersServiceClient(HttpClient httpClient, IConsulClient consulClient)
    {
        _httpClient = httpClient;
        _consulClient = consulClient;
    }

    public List<Player> GetPlayers()
    {
        var services = _consulClient.Agent.Services().Result.Response;
        var core = services.Values.FirstOrDefault(s => s.Service.Equals("core"));
        var response = _httpClient.GetStringAsync($"http://{core.Address}:{core.Port}/api/players").Result;
        var players = JsonSerializer.Deserialize<List<Player>>(response);
        return players;
    }
}


public record Player
{
    public string id { get; init; }
    public string nickname { get; init; }
    public int credits { get; init; }
    public int experience { get; init; }
    public string rank { get; init; }
    public string commandCenterId { get; init; }
}


public record ServiceDiscoveryConfig
{
    public string NameOfService { get; init; }
    public string IdOfService { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }
}
public static class ConsulBuilderExtensions
{
    public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
    {

        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        var settings = app.ApplicationServices.GetRequiredService<IOptions<ServiceDiscoveryConfig>>();

        var serviceName = settings.Value.NameOfService;
        var serviceId = settings.Value.IdOfService;
        var uri = new Uri($"http://{settings.Value.Host}:{settings.Value.Port}");

        var registration = new AgentServiceRegistration()
        {
            ID = serviceId,
            Name = serviceName,
            Address = $"{settings.Value.Host}",
            Port = uri.Port,
            Tags = new[] { $"urlprefix-/{settings.Value.IdOfService}" }
        };

        var result = consulClient.Agent.ServiceDeregister(registration.ID).Result;
        result = consulClient.Agent.ServiceRegister(registration).Result;

        lifetime.ApplicationStopping.Register(() =>
        {
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}
