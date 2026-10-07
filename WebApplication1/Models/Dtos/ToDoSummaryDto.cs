namespace WebApplication1.Models.Dtos
{
    public class ToDoSummaryDto
    {
        public int ToDoNumber { get; set; }
        public int OnCreateToDoNumber { get; set; }
        public int OnProgressToDoNumber { get;set; }
        public int OnFinishedToDoNumber { get; set; }
        public List<ToDoDto> TopThreeToDo { get; set; }
    }
}
