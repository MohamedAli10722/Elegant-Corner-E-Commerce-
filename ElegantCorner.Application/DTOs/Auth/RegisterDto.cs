namespace ElegantCorner.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}