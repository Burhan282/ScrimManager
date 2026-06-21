using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrimManagerApplication.Application.Models.DTOModels
{
    public class CreateUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Rank UserRank { get; set; }
        public Region UserRegion { get; set; }
        public Role UserRole { get; set; }
        public byte[]? UserLogo { get; set; }
        public string? Description { get; set; }
    }
}
