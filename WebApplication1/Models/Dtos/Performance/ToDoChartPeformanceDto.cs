namespace WebApplication1.Models.Dtos.Performance
{
    public class ToDoChartPeformanceDto
    {
        public List<ChartObjectDto> ToDoOnCreatedYear { get; set; }
        public List<ChartObjectDto> ToDoOnProgressedYear { get; set; }
        public List<ChartObjectDto> ToDoOnFinishedYear { get; set; }
    }
}
