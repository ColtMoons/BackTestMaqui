using Domain.Abstractions;
using Infrastructure.Database;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .AddServices()
                .AddDatabase(configuration)
                .AddRepositories()
                .AddHealthChecks(configuration);

        private IServiceCollection AddServices()
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }

        private IServiceCollection AddDatabase(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Database");

            services.AddDbContext<ApplicationDbContext>(
                options => options
                    .UseNpgsql(connectionString, npgsqlOptions =>
                        npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                    .UseSnakeCaseNamingConvention());

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            return services;
        }

        private IServiceCollection AddHealthChecks(IConfiguration configuration)
        {
            services
                .AddHealthChecks()
                .AddNpgSql(configuration.GetConnectionString("Database")!);

            return services;
        }
        
        private IServiceCollection AddRepositories()
        {
            services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
                .AddClasses(
                    classes => classes.Where(type => type.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase)),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }
    }
}
