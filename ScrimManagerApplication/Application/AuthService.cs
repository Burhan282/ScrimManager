using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Register(
            string username,
            string email,
            string password,
            Role role,
            Rank rank)
        {
            User user = new User();

            user.Username = username;
            user.Email = email;
            user.PasswordHash = password;
            user.Role = role;
            user.Rank = rank;

            _userRepository.Add(user);
        }
    }
}