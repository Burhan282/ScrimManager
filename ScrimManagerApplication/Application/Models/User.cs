namespace ScrimManagerApplication.Application.Models
{
    public class User
    {
        public int Id { get; set; }      
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role UserRole { get; set; }
        public Rank UserRank { get; set; }
        public Region UserRegion { get; set; }
        public byte[]? UserLogo { get; set; }
        public string? Description { get; set; }
    }
}
