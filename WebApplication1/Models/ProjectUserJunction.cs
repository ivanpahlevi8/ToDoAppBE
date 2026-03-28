namespace WebApplication1.Models
{
    public class ProjectUserJunction
    {
        public string UserId { get; set; }
        public UserModel User { get; set; }

        public int ProjectId { get; set; }
        public ProjectModel Project { get; set; }
    }
}
