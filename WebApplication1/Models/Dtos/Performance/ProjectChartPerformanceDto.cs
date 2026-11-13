namespace WebApplication1.Models.Dtos.Performance
{
    public class ProjectChartPerformanceDto
    {
        public List<ChartObjectDto> ProjectLeads { get; set; }
        public List<ChartObjectDto> ProjectAsMember { get; set; }
    }
}
