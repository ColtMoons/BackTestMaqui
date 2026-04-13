using Application.Abstractions.Services;
using Domain.Abstractions;
using Infrastructure.Cache;
using Infrastructure.Database;
using Infrastructure.Discounts;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

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
                .AddRefitClients(configuration)
                .AddHealthChecks(configuration);

        private IServiceCollection AddServices()
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IDiscountService, DiscountService>();

            services.AddSingleton<IStatusCacheService, StatusCacheService>();
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

        private IServiceCollection AddRefitClients(IConfiguration configuration)
        {
            services.AddRefitClient<IDiscountApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["Api:DiscountApi"]!));
            
            return services;
        }
    }
}
