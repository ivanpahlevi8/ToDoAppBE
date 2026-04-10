using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Dtos
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectUserLeadId { get; set; }
        public string ProjectStatus { get; set; }
        public int? ProjectTeamId { get; set; }
        public List<ToDoModel> ToDos { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
