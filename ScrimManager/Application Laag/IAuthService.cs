using ScrimManager.Domain;

namespace ScrimManager.Application;

public interface IAuthService
{
    User Register(string userName, string email, string password);
    User Login(string email, string password);
}
