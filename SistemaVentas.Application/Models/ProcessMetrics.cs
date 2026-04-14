namespace SistemaVentas.Application.Models
{
    public class ProcessMetrics
    {
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public int TotalSources { get; set; }
        public int TotalRecords { get; set; }
        public long TotalDurationInMilliseconds { get; set; }
        public List<ExtractionResult> Results { get; set; } = new();
    }
}