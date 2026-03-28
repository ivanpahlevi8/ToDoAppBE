using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ProjectModel
    {
        [Key]
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        // relation to many with user
        public string ProjectUserLeadId { get; set; }
        public UserModel ProjectUserLead { get; set; }

        public string ProjectStatus { get; set; }

        // relation to many with project
        public int? ProjectTeamId { get; set; }
        public TeamModel? Team { get; set; }

        public DateTime CreatedAt { get; set; }

        // relation to many with to do
        public List<ToDoModel> ToDos { get; set; }

        // relation to many with Project User Junction
        public List<ProjectUserJunction> ProjectUserJunctions { get; set; }
    }
}
