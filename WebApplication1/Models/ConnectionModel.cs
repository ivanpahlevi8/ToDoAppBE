using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ConnectionModel
    {
        [Key]
        public int ConnectionId { get; set; }

        public string UserOwnerId { get; set; }
        public UserModel? UserOwner { get; set; }

        public string UserConnectionId { get; set; }
        public UserModel? UserConnection { get; set; }

        public string ConnectionStatus { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
