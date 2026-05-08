using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Dtos
{
    public class TeamRoleDto
    {
        public int? TeamRoleId { get; set; }

        public string RoleName { get; set; }

        public int TeamId { get; set; }

        public TeamModel? Team { get; set; }

        public List<TeamUserJunction>? TeamUserJunction { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
