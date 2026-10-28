namespace WebApplication1.Models.Dtos.Performance
{
    public class ConnectionPerformanceDto
    {
        public int ConnectionConnectCurrMonth { get; set; }
        public int ConnectionConnectLastMonth { get; set; }
        public int ConnectionConnectPercentage { get; set; }
        public int ConnectionDisconnectCurrMonth { get; set; }
        public int ConnectionDisconnectLastMonth { get; set; }
        public int ConnectionDisconnectPercentage { get; set;}
    }
}
