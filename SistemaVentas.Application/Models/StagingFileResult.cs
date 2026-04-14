namespace SistemaVentas.Application.Models
{
    public class StagingFileResult
    {
        public bool WasCreated { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public int RecordsSaved { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}