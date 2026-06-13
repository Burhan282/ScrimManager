using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System;
using System.Collections.Generic;

namespace ScrimManagerDataAcces.DataAcces.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly string connectionString;

        public TeamRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int Add(Team team)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO team
                (name, region, rank, description, logo_data, created_by_user_id)
                VALUES
                (@name, @region, @rank, @description, @logo_data, @created_by_user_id)
                RETURNING id;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", team.Name);
            cmd.Parameters.AddWithValue("@region", team.Teamregion.ToString());
            cmd.Parameters.AddWithValue("@rank", team.Teamrank.ToString());
            cmd.Parameters.AddWithValue("@description", team.Description ?? "");
            cmd.Parameters.AddWithValue("@logo_data", (object?)team.LogoData ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@created_by_user_id", team.CreatedByUserId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Team> GetAll()
        {
            var teams = new List<Team>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM team ORDER BY id DESC";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                teams.Add(MapTeam(reader));
            }

            return teams;
        }

        public Team? FindById(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM team WHERE id = @id";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapTeam(reader);
            }

            return null;
        }

        public void Join(int userId, int teamId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO user_team
                (user_id, team_id)
                VALUES
                (@userId, @teamId)
                ON CONFLICT DO NOTHING;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@teamId", teamId);

            cmd.ExecuteNonQuery();
        }

        public void ApplyToTeam(int userId, int teamId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO team_join_request
                (user_id, team_id, status)
                VALUES
                (@userId, @teamId, 'Pending')
                ON CONFLICT DO NOTHING;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@teamId", teamId);

            cmd.ExecuteNonQuery();
        }

        public List<TeamJoinRequest> GetPendingRequestsForCaptain(int captainUserId)
        {
            var requests = new List<TeamJoinRequest>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT 
                    r.id,
                    r.user_id,
                    r.team_id,
                    r.status,
                    r.created_at,
                    u.username,
                    u.rank,
                    t.name AS team_name
                FROM team_join_request r
                INNER JOIN ""user"" u ON u.id = r.user_id
                INNER JOIN team t ON t.id = r.team_id
                WHERE t.created_by_user_id = @captainUserId
                  AND r.status = 'Pending'
                ORDER BY r.created_at DESC;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@captainUserId", captainUserId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                requests.Add(MapTeamJoinRequest(reader));
            }

            return requests;
        }

        public int? AcceptJoinRequest(int requestId, int captainUserId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var transaction = conn.BeginTransaction();

            string selectQuery = @"
                SELECT user_id, team_id
                FROM team_join_request request
                INNER JOIN team ON team.id = request.team_id
                WHERE request.id = @requestId
                  AND request.status = 'Pending'
                  AND team.created_by_user_id = @captainUserId;
            ";

            int userId;
            int teamId;

            using (var selectCmd = new NpgsqlCommand(selectQuery, conn, transaction))
            {
                selectCmd.Parameters.AddWithValue("@requestId", requestId);
                selectCmd.Parameters.AddWithValue("@captainUserId", captainUserId);

                using var reader = selectCmd.ExecuteReader();

                if (!reader.Read())
                {
                    transaction.Rollback();
                    return null;
                }

                userId = Convert.ToInt32(reader["user_id"]);
                teamId = Convert.ToInt32(reader["team_id"]);
            }

            string insertQuery = @"
                INSERT INTO user_team
                (user_id, team_id)
                VALUES
                (@userId, @teamId)
                ON CONFLICT DO NOTHING;
            ";

            using (var insertCmd = new NpgsqlCommand(insertQuery, conn, transaction))
            {
                insertCmd.Parameters.AddWithValue("@userId", userId);
                insertCmd.Parameters.AddWithValue("@teamId", teamId);
                insertCmd.ExecuteNonQuery();
            }

            string updateQuery = @"
                UPDATE team_join_request
                SET status = 'Accepted'
                WHERE id = @requestId;
            ";

            using (var updateCmd = new NpgsqlCommand(updateQuery, conn, transaction))
            {
                updateCmd.Parameters.AddWithValue("@requestId", requestId);
                updateCmd.ExecuteNonQuery();
            }

            transaction.Commit();

            return teamId;
        }

        public void DeclineJoinRequest(int requestId, int captainUserId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                UPDATE team_join_request request
                SET status = 'Declined'
                FROM team
                WHERE request.id = @requestId
                  AND request.status = 'Pending'
                  AND team.id = request.team_id
                  AND team.created_by_user_id = @captainUserId;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@requestId", requestId);
            cmd.Parameters.AddWithValue("@captainUserId", captainUserId);

            cmd.ExecuteNonQuery();
        }

        public List<Team> GetTeamsByUserId(int userId)
        {
            var teams = new List<Team>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT DISTINCT t.*
                FROM team t
                LEFT JOIN user_team ut ON ut.team_id = t.id
                WHERE t.created_by_user_id = @userId
                   OR ut.user_id = @userId
                ORDER BY t.id DESC;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                teams.Add(MapTeam(reader));
            }

            return teams;
        }

        public List<User> GetTeamMembers(int teamId)
        {
            var users = new List<User>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                SELECT u.id, u.username, u.email, u.password_hash, u.role, u.rank, u.region, u.user_logo, u.description
                FROM user_team ut
                INNER JOIN ""user"" u ON u.id = ut.user_id
                WHERE ut.team_id = @teamId
                ORDER BY u.username;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@teamId", teamId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        public void UpdateTeamRank(int teamId, Rank rank)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                UPDATE team
                SET rank = @rank
                WHERE id = @teamId;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@rank", rank.ToString());
            cmd.Parameters.AddWithValue("@teamId", teamId);

            cmd.ExecuteNonQuery();
        }

        private Team MapTeam(NpgsqlDataReader reader)
        {
            return new Team
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"]?.ToString() ?? "",
                Teamregion = Enum.Parse<Region>(reader["region"]?.ToString() ?? "EU"),
                Teamrank = Enum.Parse<Rank>(reader["rank"]?.ToString() ?? "BronzeI"),
                Description = reader["description"] == DBNull.Value ? "" : reader["description"]?.ToString(),
                LogoData = reader["logo_data"] == DBNull.Value ? null : (byte[])reader["logo_data"],
                CreatedByUserId = reader["created_by_user_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["created_by_user_id"])
            };
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

        private TeamJoinRequest MapTeamJoinRequest(NpgsqlDataReader reader)
        {
            return new TeamJoinRequest
            {
                Id = Convert.ToInt32(reader["id"]),
                UserId = Convert.ToInt32(reader["user_id"]),
                TeamId = Convert.ToInt32(reader["team_id"]),
                Status = reader["status"]?.ToString() ?? "Pending",
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Username = reader["username"]?.ToString() ?? "",
                TeamName = reader["team_name"]?.ToString() ?? "",

                UserRank = reader["rank"] == DBNull.Value
                    ? Rank.BronzeI
                    : (Rank)Convert.ToInt32(reader["rank"])
            };
        }
    }
}
