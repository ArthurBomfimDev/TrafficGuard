using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrafficGuard.Domain.Interfaces;
using TrafficGuard.Infrastructure.Data;
using TrafficGuard.Infrastructure.Reports;
using TrafficGuard.Infrastructure.Repositories;

namespace TrafficGuard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TrafficGuardDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IViolationRepository, ViolationRepository>();
        services.AddScoped<IViolationReportGenerator, ViolationReportGenerator>(); 

        return services;
    }
}