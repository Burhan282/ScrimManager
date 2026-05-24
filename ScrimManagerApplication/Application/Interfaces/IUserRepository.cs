using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);

        User? GetByEmailAndPassword(string email, string password);
    }
}