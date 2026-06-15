using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScrimManagerDataAcces.DataAcces.FakeDataBases
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> users = new();

        public void Add(User user)
        {
            users.Add(user);
        }

        public User? GetByEmail(string email)
        {
            return users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public List<User> GetAll()
        {
            return users;
        }
    }
}
