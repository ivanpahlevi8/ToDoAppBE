namespace WebApplication1.Models
{
    public class TeamUserJunction
    {
        public int TeamId { get; set; }
        public TeamModel Team { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; }
    }
}
