using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using weatherstation.Application.Logic;
using weatherstation.Application.LogicInterfaces;
using weatherstation.Utils;

#pragma warning disable AZFW0014 // Missing expected registration of ASP.NET Core Integration services
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IAccountLogic, AccountLogic>();
        services.AddScoped<INotificationLogic, NotificationLogic>();
        services.AddScoped<IRecommendationLogic, RecommendationLogic>();
        services.AddScoped<IWeatherLogic, WeatherLogic>();

        var sqlConnectionString = Environment.GetEnvironmentVariable("SQLCON1", EnvironmentVariableTarget.Process);
        services.AddSingleton<IDBManager>(new DBManager(sqlConnectionString));
        var host = Environment.GetEnvironmentVariable("SOCKET_IP", EnvironmentVariableTarget.Process);
        var port = Environment.GetEnvironmentVariable("SOCKET_PORT", EnvironmentVariableTarget.Process);
        services.AddSingleton<ISocketManager>(new SocketManager(host, int.Parse(port)));
    })
    .Build();
#pragma warning restore AZFW0014 // Missing expected registration of ASP.NET Core Integration services

host.Run();
