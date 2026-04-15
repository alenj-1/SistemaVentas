using System.Diagnostics;
using Microsoft.Extensions.Options;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.SourceModels.Csv;
using SistemaVentas.Application.Models;
using SistemaVentas.Infrastructure.Settings;

namespace SistemaVentas.Infrastructure.Extractors
{
    public class CsvExtractor : IExtractor
    {
        private readonly IFileReader<Customer> _customerReader;
        private readonly IFileReader<Product> _productReader;
        private readonly IFileReader<Order> _orderReader;
        private readonly IFileReader<OrderDetails> _orderDetailsReader;

        private readonly IStagingService _stagingService;
        private readonly ILoggerService _loggerService;

        private readonly CsvSettings _csvSettings;


        public string SourceName => "CSV Files";


        public CsvExtractor(
            IFileReader<Customer> customerReader,
            IFileReader<Product> productReader,
            IFileReader<Order> orderReader,
            IFileReader<OrderDetails> orderDetailsReader,
            IStagingService stagingService,
            ILoggerService loggerService,
            IOptions<CsvSettings> csvOptions)
        {
            _customerReader = customerReader;
            _productReader = productReader;
            _orderReader = orderReader;
            _orderDetailsReader = orderDetailsReader;
            _stagingService = stagingService;
            _loggerService = loggerService;
            _csvSettings = csvOptions.Value;
        }


        // Ejecuta la extracción de todos los CSV y guarda el resultado en staging
        public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var startedAt = DateTime.Now;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _loggerService.LogInformation("Starting CSV file extraction...");

                // Se construyen las rutas completas
                var customersPath = Path.Combine(_csvSettings.BasePath, _csvSettings.CustomersFile);
                var productsPath = Path.Combine(_csvSettings.BasePath, _csvSettings.ProductsFile);
                var ordersPath = Path.Combine(_csvSettings.BasePath, _csvSettings.OrdersFile);
                var orderDetailsPath = Path.Combine(_csvSettings.BasePath, _csvSettings.OrderDetailsFile);

                // Se leen todos los CSV
                var customers = await _customerReader.ReadAsync(customersPath, cancellationToken);
                var products = await _productReader.ReadAsync(productsPath, cancellationToken);
                var orders = await _orderReader.ReadAsync(ordersPath, cancellationToken);
                var orderDetails = await _orderDetailsReader.ReadAsync(orderDetailsPath, cancellationToken);


                // Se crea un objeto anónimo con toda la extracción
                var extractedData = new
                {
                    Customers = customers,
                    Products = products,
                    Orders = orders,
                    OrderDetails = orderDetails
                };

                // Se guarda el resultado en staging
                var stagingResult = await _stagingService.SaveAsync(extractedData, "csv_extraction.json", cancellationToken);

                stopwatch.Stop();

                var totalRecords = customers.Count + products.Count + orders.Count + orderDetails.Count;

                _loggerService.LogInformation("CSV extraction completed successfully.");
                return new ExtractionResult
                {
                    SourceName = SourceName,
                    WasSuccessful = true,
                    RecordsExtracted = totalRecords,
                    Message = "CSV extraction completed successfully.",
                    StagingFilePath = stagingResult.FilePath,
                    StartedAt = startedAt,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _loggerService.LogError("Error during CSV file extraction.", ex);

                return new ExtractionResult
                {
                    SourceName = SourceName,
                    WasSuccessful = false,
                    RecordsExtracted = 0,
                    Message = ex.Message,
                    StartedAt = startedAt,
                    EndedAt = DateTime.Now,
                    DurationInMilliseconds = stopwatch.ElapsedMilliseconds
                };
            }
        }
    }
}