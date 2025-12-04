using System.Security.Cryptography;
using System.Text;
using ScrimManager.Data;
using ScrimManager.Domain;

namespace ScrimManager.Application;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Register(string userName, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Gebruikersnaam is verplicht.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mailadres is verplicht.");

        if (!email.Contains("@"))
            throw new ArgumentException("E-mailadres is ongeldig.");

        if (password.Length < 8)
            throw new ArgumentException("Wachtwoord moet minimaal 8 tekens lang zijn.");

        // Controleer of e-mail al bestaat
        if (_userRepository.GetByEmail(email) != null)
            throw new InvalidOperationException("Dit e-mailadres is al in gebruik.");

        var user = new User
        {
            UserName = userName,
            Email = email,
            PasswordHash = HashPassword(password)
        };

        _userRepository.Add(user);

        return user;
    }

 
    public User Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("E-mailadres en wachtwoord zijn verplicht.");

        var user = _userRepository.GetByEmail(email);

        if (user == null)
            throw new InvalidOperationException("Gebruiker bestaat niet.");

        var passwordHash = HashPassword(password);

        if (user.PasswordHash != passwordHash)
            throw new InvalidOperationException("Wachtwoord is onjuist.");

        return user;
    }

    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
