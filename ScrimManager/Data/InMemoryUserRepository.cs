using System.Collections.Generic;
using System.Linq;
using ScrimManager.Domain;

namespace ScrimManager.Data;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public User? GetByEmail(string email)
    {
        return _users.SingleOrDefault(u => u.Email == email);
    }

    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
    }

}
