using TrafficGuard.Application;
using TrafficGuard.Infrastructure;
using TrafficGuard.Infrastructure.Data;
using TrafficGuard.Worker;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure; 

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

QuestPDF.Settings.License = LicenseType.Community; 

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddApplicationServices();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddInfrastructureServices(connectionString);

        services.AddHostedService<RabbitMqWorker>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<TrafficGuardDbContext>();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine(" [DB] Database ready.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [DB ERROR] {ex.Message}");
    }
}

await host.RunAsync();