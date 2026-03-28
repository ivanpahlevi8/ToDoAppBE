using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Dtos
{
    public class TeamDto
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public string TeamDescription { get; set; }
        public string TeamLeader { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
