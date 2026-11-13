namespace WebApplication1.Models.Dtos.Performance
{
    public class ConnectionChartPeformanceDto
    {
        public List<ChartObjectDto> ConnectionConnectedChart {  get; set; }
        public List<ChartObjectDto> ConnectionDisconnectedChart { get; set; }
    }
}
