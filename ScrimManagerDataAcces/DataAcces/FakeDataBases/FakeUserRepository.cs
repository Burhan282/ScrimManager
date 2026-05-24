using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerDataAcces.DataAcces.FakeDataBases
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> users = new();

        public void Add(User user)
        {
            users.Add(user);
        }

        public User? GetByEmailAndPassword(string email, string password)
        {
            return users.FirstOrDefault(u =>
                u.Email == email &&
                u.PasswordHash == password);
        }

        public List<User> GetAll()
        {
            return users;
        }
    }
}