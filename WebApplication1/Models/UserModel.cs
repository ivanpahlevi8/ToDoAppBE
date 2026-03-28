using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class UserModel : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }

        // relation to many with teams
        public List<TeamModel> Teams { get; set; }

        // relation to many with projects
        public List<ProjectModel> Projects { get; set; }

        // relation to many with ProjectUserJunction
        public List<ProjectUserJunction> ProjectUsersJunction { get; set; }

        // relation to many with TeamUserJunction
        public List<TeamUserJunction> TeamUsersJunction { get; set;}
    }
}
