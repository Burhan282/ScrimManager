using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                (name, region, rank) VALUES
                (@name, @region, @rank)";


            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", team.Name);
            cmd.Parameters.AddWithValue("@region", team.Teamregion);
            cmd.Parameters.AddWithValue("@rank", team.Teamrank);

            cmd.ExecuteNonQuery();

        }

        public List<Team> GetAll()
        {
            List<Team> team = new List<Team>();

            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM team";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                team.Add(MapTeam(reader));
            }

            return team;
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
                (@userId, @teamId";

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

                Teamregion = Enum.Parse<Region>(
                    reader["region"].ToString() ?? "EU"),

                Teamrank = (Rank)Convert.ToInt32(reader["rank"])
            };
        }







    }


}