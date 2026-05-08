using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TeamRoleModel
    {
        [Key]
        public int TeamRoleId { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        public int TeamId { get; set; }

        public TeamModel Team { get; set; }

        public List<TeamUserJunction> TeamUserJunction {  get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
