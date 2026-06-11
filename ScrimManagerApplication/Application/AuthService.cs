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
            User user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = password,
                Role = roles,
                UserRank = rank
            };

            _userRepository.Add(user);
        }

        
        public User? Login(string email, string password)
        {
            var user = _userRepository.GetByEmailAndPassword(email, password);

            
            if (user != null)
            {
                System.Diagnostics.Debug.WriteLine($"LOGIN OK - USER ID: {user.Id}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LOGIN FAILED");
            }

            return user;
        }
    }
}