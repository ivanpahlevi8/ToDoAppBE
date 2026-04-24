namespace WebApplication1.Models.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
