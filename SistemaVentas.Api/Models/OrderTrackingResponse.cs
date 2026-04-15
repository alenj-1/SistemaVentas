namespace SistemaVentas.Api.Models
{
    public class OrderTrackingResponse
    {
        public int TrackingId { get; set; }
        public int OrderId { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string TrackingStatus { get; set; } = string.Empty;
        public DateTime LastUpdate { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCountry { get; set; } = string.Empty;
    }
}