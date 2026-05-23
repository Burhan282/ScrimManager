using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);
    }
}