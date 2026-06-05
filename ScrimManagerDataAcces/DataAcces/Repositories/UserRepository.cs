using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerDataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(User user)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO ""user""
                (username, email, password_hash, role, rank)
                VALUES
                (@username, @email, @passwordHash, @role, @rank)
            ";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@role", user.Role);
            cmd.Parameters.AddWithValue("@rank", (int)user.Rank);

            cmd.ExecuteNonQuery();
        }

        public User? GetByEmailAndPassword(string email, string password)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT *
                FROM ""user""
                WHERE email = @email
                AND password_hash = @password
            ";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            using NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Username = reader["username"].ToString() ?? "",
                    Email = reader["email"].ToString() ?? "",
                    PasswordHash = reader["password_hash"].ToString() ?? "",
                    Role = reader["role"].ToString() ?? "",
                    Rank = (Rank)Convert.ToInt32(reader["rank"])
                };
            }

            return null;
        }
    }
}