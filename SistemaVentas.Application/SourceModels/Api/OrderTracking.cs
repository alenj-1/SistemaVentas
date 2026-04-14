namespace SistemaVentas.Application.SourceModels.Api
{
    public class OrderTracking
    {
        public int TrackingID { get; set; }
        public int OrderID { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string TrackingStatus { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCountry { get; set; } = string.Empty;
    }
}