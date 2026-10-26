namespace WebApplication1.Models.Dtos.Performance
{
    public class ToDoPerformanceDto
    {
        public int NumberToDoCreatedThisMonth { get; set; }
        public int NumberToDoCreatedLastMonth { get; set; }
        public int PercentageToDoCreated {  get; set; }
        public int NumberToDoOnProgressThisMonth { get; set; }
        public int NumberToDoOnProgressLastMonth { get; set; }
        public int PercentageToDoOnProgress {  get; set; }
        public int NumberToDoDoneThisMonth { get; set; }
        public int NumberToDoDoneLastMonth { get; set; }
        public int PercentageToDoDone { get; set; }
    }
}
