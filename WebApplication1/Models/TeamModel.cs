using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TeamModel
    {
        [Key]
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public string TeamDescription { get; set; }

        // to one with user as team leader
        public string TeamLeader {  get; set; }
        public UserModel TeamUserLeader { get; set; }

        public DateTime CreatedAt { get; set; }

        // relation to many with project
        public List<ProjectModel> Projects { get; set; }

        // relation to many with ProjectUserJunction
        public List<TeamUserJunction> TeamUserJunction { get; set;}
    }
}
