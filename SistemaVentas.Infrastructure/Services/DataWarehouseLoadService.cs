using Microsoft.EntityFrameworkCore;
using SistemaVentas.Application.Common;
using SistemaVentas.Application.Interfaces;
using SistemaVentas.Application.Models;
using SistemaVentas.Application.SourceModels.Csv;
using SistemaVentas.Persistence.Context;
using SistemaVentas.Persistence.Entities.Datawarehouse.Dimensions;
using SistemaVentas.Persistence.Entities.Datawarehouse.Facts;
using System.Text.Json;

namespace SistemaVentas.Infrastructure.Services
{
    public class DataWarehouseLoadService : IDataWarehouseLoadService
    {
        private readonly DataWarehouseDbContext _dataWarehouseContext;

        private readonly ILoggerService _loggerService;

        public DataWarehouseLoadService(DataWarehouseDbContext dataWarehouseContext, ILoggerService loggerService)
        {
            _dataWarehouseContext = dataWarehouseContext;
            _loggerService = loggerService;
        }
        

        // Carga todas las dimensiones
        public async Task<LoadProcessResult> LoadDimensionsAsync(string stagingFilePath, CancellationToken cancellationToken = default)
        {
            var result = new LoadProcessResult
            {
                ProcessName = "Load Dimensions"
            };

            try
            {
                _loggerService.LogInformation("Starting load of Dimensions...");

                var extractedData = await ReadStagingAsync(stagingFilePath, cancellationToken);

                await using var transaction = await _dataWarehouseContext.Database.BeginTransactionAsync(cancellationToken);

                await ClearFactSalesAsync(cancellationToken);
                await ClearDimensionsAsync(cancellationToken);

                // Se cargan las dimensiones
                result.TotalCategories = await LoadCategoriesAsync(extractedData.Products, cancellationToken);
                result.TotalProducts = await LoadProductsAsync(extractedData.Products, cancellationToken);
                result.TotalLocations = await LoadLocationsAsync(extractedData.Customers, cancellationToken);
                result.TotalCustomers = await LoadCustomersAsync(extractedData.Customers, cancellationToken);
                result.TotalStatuses = await LoadOrderStatusesAsync(extractedData.Orders, cancellationToken);
                result.TotalDates = await LoadDatesAsync(extractedData.Orders, cancellationToken);

                // Se confirma la transacción
                await transaction.CommitAsync(cancellationToken);

                result.WasSuccessful = true;
                result.Message = "Dimension load completed successfully.";

                _loggerService.LogInformation("Error during Dimension loading.");

            }
            catch (Exception ex)
            {
                result.WasSuccessful = false;
                result.Message = ex.Message;

                _loggerService.LogError("Error during Dimension loading.", ex);
            }

            return result;
        }


        // Carga la tabla de hechos FactSales
        public async Task<LoadProcessResult> LoadFactsAsync(string stagingFilePath, CancellationToken cancellationToken = default)
        {
            var result = new LoadProcessResult
            {
                ProcessName = "Load FactSales"
            };

            try
            {
                _loggerService.LogInformation("Starting load of FactSales...");

                var extractedData = await ReadStagingAsync(stagingFilePath, cancellationToken);

                await using var transaction = await _dataWarehouseContext.Database.BeginTransactionAsync(cancellationToken);

                await ClearFactSalesAsync(cancellationToken);

                result.TotalFactSales = await LoadFactsAsync(
                    extractedData.Customers,
                    extractedData.Products,
                    extractedData.Orders,
                    extractedData.OrderDetails,
                    cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                result.WasSuccessful = true;
                result.Message = "FactSales load completed successfully.";
            }
            catch (Exception ex)
            {
                result.WasSuccessful = false;
                result.Message = ex.Message;
                _loggerService.LogError("Error during FactSales loading.", ex);
            }
            return result;
        }


        // Lee el archivo de staging y lo convierte al modelo de trabajo
        private async Task<CsvExtractionStage> ReadStagingAsync(string stagingFilePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(stagingFilePath) || !File.Exists(stagingFilePath))
            {
                throw new FileNotFoundException("Staging file not found.", stagingFilePath);
            }

            // Se lee el contenido del archivo JSON
            var json = await File.ReadAllTextAsync(stagingFilePath, cancellationToken);

            var extractedData = JsonSerializer.Deserialize<CsvExtractionStage>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return extractedData ?? new CsvExtractionStage();
        }


        // Limpia solamente la tabla de hechos FactSales
        private async Task ClearFactSalesAsync(CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Cleaning FactSales...");

            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM FactSales", cancellationToken);

            _loggerService.LogInformation("FactSales cleaned successfully.");
        }


        // Limpia solamente las Dimensiones
        private async Task ClearDimensionsAsync(CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Cleaning tables...");

            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimCustomer", cancellationToken);
            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimProduct", cancellationToken);
            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimDate", cancellationToken);
            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimOrderStatus", cancellationToken);
            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimLocation", cancellationToken);
            await _dataWarehouseContext.Database.ExecuteSqlRawAsync("DELETE FROM DimCategory", cancellationToken);

            _loggerService.LogInformation("Dimensions cleaned successfully.");
        }


        // Carga la dimensión de categorías
        private async Task<int> LoadCategoriesAsync(List<Product> products, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimCategory...");

            var categories = products
                .Where(x => !string.IsNullOrWhiteSpace(x.Category))
                .GroupBy(x => NormalizeText(x.Category))
                .Select(g => new DimCategory
                {
                    CategoryName = g.Key
                })
                .ToList();

            await _dataWarehouseContext.DimCategory.AddRangeAsync(categories, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return categories.Count;
        }


        // Carga la dimensión de productos
        private async Task<int> LoadProductsAsync(List<Product> products, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimProduct...");

            // Se crea un diccionario para buscar rápido la llave de cada categoría
            var categoryDictionary = await _dataWarehouseContext.DimCategory
                .AsNoTracking()
                .ToDictionaryAsync(x => x.CategoryName, x => x.CategoryKey, cancellationToken);

            // Se convierten los productos CSV a productos del DW
            var dimProducts = products.Select(x => new DimProduct
            {
                ProductID_NaturalKey = x.ProductID,
                ProductName = NormalizeText(x.ProductName),
                CategoryKey = categoryDictionary.TryGetValue(NormalizeText(x.Category), out var categoryKey) ? categoryKey : null,
                ListPrice = x.Price,
                Stock = x.Stock
            }).ToList();

            await _dataWarehouseContext.DimProduct.AddRangeAsync(dimProducts, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return dimProducts.Count;
        }


        // Carga la dimensión de ubicación
        private async Task<int> LoadLocationsAsync(List<Customer> customers, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimLocation...");

            // Se toman combinaciones únicas de país y ciudad
            var locations = customers
                .Where(x => !string.IsNullOrWhiteSpace(x.Country) && !string.IsNullOrWhiteSpace(x.City))
                .GroupBy(x => new
                {
                    Country = NormalizeText(x.Country),
                    City = NormalizeText(x.City)
                })
                .Select(g => new DimLocation
                {
                    Country = g.Key.Country,
                    City = g.Key.City
                })
                .ToList();

            await _dataWarehouseContext.DimLocation.AddRangeAsync(locations, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return locations.Count;
        }


        // Carga la dimensión de clientes
        private async Task<int> LoadCustomersAsync(List<Customer> customers, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimCustomer...");

            // Diccionario para buscar la ubicación de cada cliente
            var locationDictionary = await _dataWarehouseContext.DimLocation
                .AsNoTracking()
                .ToDictionaryAsync(
                    x => BuildLocationKey(x.Country, x.City),
                    x => x.LocationKey,
                    cancellationToken);

            var dimCustomers = customers.Select(x => new DimCustomer
            {
                CustomerID_NaturalKey = x.CustomerID,
                FirstName = NormalizeText(x.FirstName),
                LastName = NormalizeText(x.LastName),
                Email = string.IsNullOrWhiteSpace(x.Email) ? null : x.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(x.Phone) ? null : x.Phone.Trim(),
                LocationKey = locationDictionary.TryGetValue(
                    BuildLocationKey(x.Country, x.City),
                    out var locationKey) ? locationKey : null
            }).ToList();

            await _dataWarehouseContext.DimCustomer.AddRangeAsync(dimCustomers, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return dimCustomers.Count;
        }


        // Carga la dimensión de estados de orden
        private async Task<int> LoadOrderStatusesAsync(List<Order> orders, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimOrderStatus...");

            var statuses = orders
                .Where(x => !string.IsNullOrWhiteSpace(x.Status))
                .GroupBy(x => NormalizeText(x.Status))
                .Select(g => new DimOrderStatus
                {
                    StatusName = g.Key
                })
                .ToList();

            await _dataWarehouseContext.DimOrderStatus.AddRangeAsync(statuses, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return statuses.Count;
        }


        // Carga la dimensión de fechas
        private async Task<int> LoadDatesAsync(List<Order> orders, CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading DimDate...");

            // Se toman fechas únicas
            var dates = orders
                .Select(x => x.OrderDate.Date)
                .Distinct()
                .OrderBy(x => x)
                .Select(date => new DimDate
                {
                    DateKey = DateKeyHelper.GetDateKey(date),
                    Date = date,
                    Year = (short)date.Year,
                    Quarter = (byte)(((date.Month - 1) / 3) + 1),
                    Month = (byte)date.Month,
                    MonthName = date.ToString("MMMM"),
                    DayOfMonth = (byte)date.Day,
                    DayName = date.ToString("dddd")
                })
                .ToList();

            await _dataWarehouseContext.DimDate.AddRangeAsync(dates, cancellationToken);
            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return dates.Count;
        }


        // Carga la tabla de hechos de ventas
        private async Task<int> LoadFactsAsync(
            List<Customer> customers,
            List<Product> products,
            List<Order> orders,
            List<OrderDetails> orderDetails,
            CancellationToken cancellationToken)
        {
            _loggerService.LogInformation("Loading FactSales...");

            // Diccionario de órdenes por OrderID
            var orderDictionary = orders.ToDictionary(x => x.OrderID, x => x);

            // Diccionario de clientes CSV por CustomerID
            var customerCsvDictionary = customers.ToDictionary(x => x.CustomerID, x => x);

            // Diccionarios de dimensiones para obtener llaves sustitutas
            var customerDictionary = await _dataWarehouseContext.DimCustomer
                .AsNoTracking()
                .ToDictionaryAsync(x => x.CustomerID_NaturalKey, x => x.CustomerKey, cancellationToken);

            var productDictionary = await _dataWarehouseContext.DimProduct
                .AsNoTracking()
                .ToDictionaryAsync(x => x.ProductID_NaturalKey, x => x.ProductKey, cancellationToken);

            var statusDictionary = await _dataWarehouseContext.DimOrderStatus
                .AsNoTracking()
                .ToDictionaryAsync(x => x.StatusName, x => x.StatusKey, cancellationToken);

            var locationDictionary = await _dataWarehouseContext.DimLocation
                .AsNoTracking()
                .ToDictionaryAsync(
                    x => BuildLocationKey(x.Country, x.City),
                    x => x.LocationKey,
                    cancellationToken);

            // Lista final de hechos
            var facts = new List<FactSales>();

            foreach (var detail in orderDetails)
            {
                if (!orderDictionary.TryGetValue(detail.OrderID, out var order))
                {
                    continue;
                }

                if (!customerDictionary.TryGetValue(order.CustomerID, out var customerKey))
                {
                    continue;
                }

                if (!productDictionary.TryGetValue(detail.ProductID, out var productKey))
                {
                    continue;
                }

                if (!statusDictionary.TryGetValue(NormalizeText(order.Status), out var statusKey))
                {
                    continue;
                }

                var dateKey = DateKeyHelper.GetDateKey(order.OrderDate);

                int? locationKey = null;

                if (customerCsvDictionary.TryGetValue(order.CustomerID, out var customerCsv))
                {
                    var customerLocationKey = BuildLocationKey(customerCsv.Country, customerCsv.City);

                    if (locationDictionary.TryGetValue(customerLocationKey, out var foundLocationKey))
                    {
                        locationKey = foundLocationKey;
                    }
                }

                var unitPrice = detail.Quantity > 0
                    ? Math.Round(detail.TotalPrice / detail.Quantity, 2) : 0;

                facts.Add(new FactSales
                {
                    DateKey = dateKey,
                    CustomerKey = customerKey,
                    ProductKey = productKey,
                    LocationKey = locationKey,
                    StatusKey = statusKey,
                    OrderID = detail.OrderID,
                    Quantity = detail.Quantity,
                    UnitPrice = unitPrice,
                    SalesAmount = detail.TotalPrice
                });
            }

            await _dataWarehouseContext.FactSales.AddRangeAsync(facts, cancellationToken);

            await _dataWarehouseContext.SaveChangesAsync(cancellationToken);

            return facts.Count;
        }


        // Limpia espacios en blanco y evita nulos
        private static string NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }


        // Construye una llave simple para buscar ubicaciones
        private static string BuildLocationKey(string? country, string? city)
        {
            return $"{NormalizeText(country)}|{NormalizeText(city)}";
        }
    }
}