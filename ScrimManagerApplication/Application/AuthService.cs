using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;

namespace ScrimManagerApplication.Application
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        
        public void Register(CreateUserDTO dto)
            
        {
            User user = new User
            {
               Username = dto.Username,
               Email = dto.Email,
               PasswordHash = dto.PasswordHash,
               UserRegion = dto.UserRegion,
               UserRole = dto.UserRole,
               UserRank = dto.UserRank,
               UserLogo = dto.UserLogo

            };

            _userRepository.Add(user);
        }

        
        public User? Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);

            if (user?.PasswordHash == password)
            {
                System.Diagnostics.Debug.WriteLine($"LOGIN OK - USER ID: {user.Id}");
                return user;
            }

            System.Diagnostics.Debug.WriteLine("LOGIN FAILED");
            return null;
        }
    }
}
