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

        public void Add(Team team)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO team
                (name, region, rank, description, logo_data, created_by_user_id)
                VALUES
                (@name, @region, @rank, @description, @logo_data, @created_by_user_id)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@name", team.Name);
            cmd.Parameters.AddWithValue("@region", team.Teamregion.ToString());
            cmd.Parameters.AddWithValue("@rank", team.Teamrank.ToString());
            cmd.Parameters.AddWithValue("@description", team.Description ?? "");
            cmd.Parameters.AddWithValue("@logo_data", (object?)team.LogoData ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@created_by_user_id", team.CreatedByUserId);

            cmd.ExecuteNonQuery();
        }

        public List<Team> GetAll()
        {
            List<Team> teams = new List<Team>();

            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM team ORDER BY id DESC";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                teams.Add(MapTeam(reader));
            }

            return teams;
        }

        public Team? FindById(int id)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM team WHERE id = @id";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapTeam(reader);
            }

            return null;
        }

        public void Join(int userId, int teamId)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO user_team
                (user_id, team_id)
                VALUES
                (@userId, @teamId)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@teamId", teamId);

            cmd.ExecuteNonQuery();
        }

        private Team MapTeam(NpgsqlDataReader reader)
        {
            return new Team
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = reader["name"].ToString() ?? "",
                Teamregion = Enum.Parse<Region>(reader["region"].ToString() ?? "EU"),
                Teamrank = Enum.Parse<Rank>(reader["rank"].ToString() ?? "Gold"),
                Description = reader["description"] == DBNull.Value ? "" : reader["description"].ToString(),
                LogoData = reader["logo_data"] == DBNull.Value ? null : (byte[])reader["logo_data"],
                CreatedByUserId = reader["created_by_user_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["created_by_user_id"])
            };
        }
    }
}