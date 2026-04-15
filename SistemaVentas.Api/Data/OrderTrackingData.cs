using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data
{
    public static class OrderTrackingData
    {
        // Método que permite devolver una lista de tracking de pedidos
        public static List<OrderTrackingResponse> GetOrderTracking()
        {
            // Lista simulada de seguimiento de órdenes
            return new List<OrderTrackingResponse>
            {
                new OrderTrackingResponse
                {
                    TrackingId = 1,
                    OrderId = 1001,
                    Carrier = "DHL",
                    TrackingStatus = "In Transit",
                    LastUpdate = DateTime.Now.AddHours(-5),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(2),
                    DestinationCity = "Santo Domingo",
                    DestinationCountry = "Dominican Republic"
                },

                new OrderTrackingResponse
                {
                    TrackingId = 2,
                    OrderId = 1002,
                    Carrier = "FedEx",
                    TrackingStatus = "Delivered",
                    LastUpdate = DateTime.Now.AddHours(-10),
                    EstimatedDeliveryDate = DateTime.Now,
                    DestinationCity = "Santiago",
                    DestinationCountry = "Dominican Republic"
                },

                new OrderTrackingResponse
                {
                    TrackingId = 3,
                    OrderId = 1003,
                    Carrier = "UPS",
                    TrackingStatus = "Pending Pickup",
                    LastUpdate = DateTime.Now.AddHours(-2),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(3),
                    DestinationCity = "La Vega",
                    DestinationCountry = "Dominican Republic"
                },

                new OrderTrackingResponse
                {
                    TrackingId = 4,
                    OrderId = 1004,
                    Carrier = "DHL",
                    TrackingStatus = "Out for Delivery",
                    LastUpdate = DateTime.Now.AddMinutes(-45),
                    EstimatedDeliveryDate = DateTime.Now,
                    DestinationCity = "San Pedro de Macorís",
                    DestinationCountry = "Dominican Republic"
                },

                new OrderTrackingResponse
                {
                    TrackingId = 5,
                    OrderId = 1005,
                    Carrier = "FedEx",
                    TrackingStatus = "In Transit",
                    LastUpdate = DateTime.Now.AddHours(-7),
                    EstimatedDeliveryDate = DateTime.Now.AddDays(1),
                    DestinationCity = "Puerto Plata",
                    DestinationCountry = "Dominican Republic"
                }
            };
        }
    }
}