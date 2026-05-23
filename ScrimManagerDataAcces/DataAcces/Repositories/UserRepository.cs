using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerDataAccess
{
    public class UserRepository : IUserRepository
    {
        private string connectionString =
            "Host=localhost;Port=5432;Database=Scrim_Manager;Username=postgres;Password=7434;";

        public void Add(User user)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO ""user""
                    (username, email, password_hash, role, rank)
                    VALUES
                    (@username, @email, @passwordHash, @role, @rank)
                ";

                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);

                cmd.Parameters.AddWithValue("@role", user.Role.ToString());

                cmd.Parameters.AddWithValue("@rank", (int)user.Rank);

                cmd.ExecuteNonQuery();
            }
        }
    }
}