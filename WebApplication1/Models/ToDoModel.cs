using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ToDoModel
    {
        [Key]
        public int ToDoId { get; set; }

        // relation to one with project
        public int ProjectId { get; set; }

        public ProjectModel Project { get; set; }

        public string ItemName { get; set; }

        public string ItemDescription { get; set; }

        public string ItemState { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
