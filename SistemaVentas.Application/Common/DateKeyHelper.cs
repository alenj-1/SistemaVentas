namespace SistemaVentas.Application.Common
{
    public static class DateKeyHelper
    {
        public static int GetDateKey(DateTime date)
        {
            return int.Parse(date.ToString("yyyyMMdd"));
        }
    }
}