using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.Dtos
{
    public class ConnectionDto
    {
        public int? ConnectionId { get; set; }

        public string UserOwnerId { get; set; }

        public string UserConnectionId { get; set; }

        public string? ConnectionStatus { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
