using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Dtos
{
    public class ToDoDto
    {
        public int ToDoId { get; set; }

        public int ProjectId { get; set; }

        public string ItemName { get; set; }

        public string ItemDescription { get; set; }

        public string ItemState { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
