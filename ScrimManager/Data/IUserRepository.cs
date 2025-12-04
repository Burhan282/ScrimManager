using ScrimManager.Domain;

namespace ScrimManager.Data;

public interface IUserRepository
{
    User? GetByEmail(string email);
    void Add(User user);
}
