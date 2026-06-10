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
                INSERT INTO ""user"" (username, email, password_hash, role, rank)
                VALUES (@username, @email, @password_hash, @role, @rank);
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("username", user.Username ?? "");
            cmd.Parameters.AddWithValue("email", user.Email ?? "");
            cmd.Parameters.AddWithValue("password_hash", user.PasswordHash ?? "");
            cmd.Parameters.AddWithValue("role", user.Role ?? "");
            cmd.Parameters.AddWithValue("rank", (int)user.UserRank);

            cmd.ExecuteNonQuery();
        }

       
        public User? GetByEmailAndPassword(string email, string password)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT id, username, email, password_hash, role, rank
                FROM ""user""
                WHERE email = @email
                LIMIT 1;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.Add("@email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = email;

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            string dbPassword = reader["password_hash"]?.ToString() ?? "";

            if (dbPassword != password)
            {
                return null;
            }

            return new User
            {
                Id = Convert.ToInt32(reader["id"]),
                Username = reader["username"]?.ToString() ?? "",
                Email = reader["email"]?.ToString() ?? "",
                PasswordHash = dbPassword,
                Role = reader["role"]?.ToString() ?? "",
                UserRank = (Rank)Convert.ToInt32(reader["rank"])
            };
        }
    }
}