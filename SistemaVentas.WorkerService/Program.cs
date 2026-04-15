using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.Services;
using SistemaVentas.Infrastructure.DependencyInjection;
using SistemaVentas.Persistence.Context;

namespace SistemaVentas.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<Worker>();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<EtlOrchestrator>();
            builder.Services.AddDbContextPool<DataWarehouseDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataWarehouseConnection")));

            var host = builder.Build();
            host.Run();
        }
    }
}