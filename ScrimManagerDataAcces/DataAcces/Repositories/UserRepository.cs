using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System;

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
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO ""user""
                (username, email, password_hash, role, rank, region, user_logo, description)
                VALUES
                (@username, @email, @password_hash, @role, @rank, @region, @user_logo, @description);
            ";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("username", user.Username ?? "");
            cmd.Parameters.AddWithValue("email", user.Email ?? "");
            cmd.Parameters.AddWithValue("password_hash", user.PasswordHash ?? "");
            cmd.Parameters.AddWithValue("role", user.UserRole.ToString());
            cmd.Parameters.AddWithValue("rank", (int)user.UserRank);
            cmd.Parameters.AddWithValue("region", user.UserRegion.ToString());
            cmd.Parameters.AddWithValue("user_logo", (object?)user.UserLogo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("description", user.Description ?? "");

            cmd.ExecuteNonQuery();
        }

        public User? GetByEmail(string email)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT id, username, email, password_hash, role, rank, region, user_logo, description
                FROM ""user""
                WHERE email = @email
                LIMIT 1;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("email", email);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapUser(reader);
        }

        public User? GetById(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT id, username, email, password_hash, role, rank, region, user_logo, description
                FROM ""user""
                WHERE id = @id
                LIMIT 1;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return MapUser(reader);
        }

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["id"]),
                Username = reader["username"]?.ToString() ?? "",
                Email = reader["email"]?.ToString() ?? "",
                PasswordHash = reader["password_hash"]?.ToString() ?? "",

                UserRole = Enum.TryParse<Role>(reader["role"]?.ToString(), out var role)
                    ? role
                    : default,

                UserRank = reader["rank"] == DBNull.Value
                    ? default
                    : (Rank)Convert.ToInt32(reader["rank"]),

                UserRegion = Enum.TryParse<Region>(reader["region"]?.ToString(), out var region)
                    ? region
                    : default,

                UserLogo = reader["user_logo"] == DBNull.Value
                    ? null
                    : (byte[])reader["user_logo"],

                Description = reader["description"]?.ToString()
            };
        }
    }
}
