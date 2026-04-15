using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.SourceModels.Csv;
using SistemaVentas.Application.SourceModels.Api;
using SistemaVentas.Infrastructure.ApiClients;
using SistemaVentas.Infrastructure.Extractors;
using SistemaVentas.Infrastructure.Readers;
using SistemaVentas.Infrastructure.Services;
using SistemaVentas.Infrastructure.Settings;

namespace SistemaVentas.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuraciones desde appsettings.json
            services.Configure<CsvSettings>(configuration.GetSection("CsvSettings"));
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
            services.Configure<StagingSettings>(configuration.GetSection("StagingSettings"));


            // También se registran como objetos directos para clases que lo usen sin IOptions
            var csvSettings = configuration.GetSection("CsvSettings").Get<CsvSettings>() ?? new CsvSettings();
            var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>() ?? new DatabaseSettings();
            var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();
            var stagingSettings = configuration.GetSection("StagingSettings").Get<StagingSettings>() ?? new StagingSettings();

            services.AddSingleton(csvSettings);
            services.AddSingleton(databaseSettings);
            services.AddSingleton(apiSettings);
            services.AddSingleton(stagingSettings);


            // Registro de los lectores CSV para cada tipo de archivo
            services.AddScoped<IFileReader<Customer>, CsvReader<Customer>>();
            services.AddScoped<IFileReader<Product>, CsvReader<Product>>();
            services.AddScoped<IFileReader<Order>, CsvReader<Order>>();
            services.AddScoped<IFileReader<OrderDetails>, CsvReader<OrderDetails>>();

            // Registro del lector de base de datos
            services.AddScoped<IDatabaseReader<Dictionary<string, object?>>>(_ =>
                new DatabaseReader(databaseSettings.ConnectionString));

            // Registro del cliente HTTP para la API
            services.AddHttpClient<IApiClient<OrderTracking>, OrderTrackingApiClient>();

            // Registro de servicios generales
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IStagingService, FileStagingService>();

            // Registro de los extractores
            services.AddScoped<IExtractor, CsvExtractor>();
            services.AddScoped<IExtractor, DatabaseExtractor>();
            services.AddScoped<IExtractor, ApiExtractor>();

            return services;
        }
    }
}