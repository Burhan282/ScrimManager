using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        User? GetById(int id);
        User? GetByEmailAndPassword(string email, string password);
    }
}