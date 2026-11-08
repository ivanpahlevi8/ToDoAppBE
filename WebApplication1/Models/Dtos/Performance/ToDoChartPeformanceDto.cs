namespace WebApplication1.Models.Dtos.Performance
{
    public class ToDoChartPeformanceDto
    {
        public List<int> ToDoOnCreatedYear { get; set; }
        public List<int> ToDoOnProgressedYear { get; set; }
        public List<int> ToDoOnFinishedYear { get; set; }
    }
}
