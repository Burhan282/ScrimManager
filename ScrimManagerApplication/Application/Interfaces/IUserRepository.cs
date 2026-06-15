using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
        User? GetById(int id);
        User? GetByEmail(string email);
    }
}
