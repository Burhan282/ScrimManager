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
            string roles,
            Rank rank)
        {
            User user = new User();

            user.Username = username;
            user.Email = email;
            user.PasswordHash = password;
            user.Role = roles;
            user.UserRank = rank;

            _userRepository.Add(user);
        }

        public User? Login(string email, string password)
        {
            return _userRepository.GetByEmailAndPassword(email, password);
        }
    }
}