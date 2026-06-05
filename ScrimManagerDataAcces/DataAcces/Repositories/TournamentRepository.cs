using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerDataAccess
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly string connectionString;

        public TournamentRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(Tournament tournament)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"INSERT INTO tournament
            (name, organizer, date, game_format, max_teams, status, description, prize_money, participating_teams)
            VALUES
            (@name, @organizer, @date, @gameFormat, @maxTeams, @status, @description, @prizeMoney, @participatingTeams)";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@name", tournament.Naam);
            cmd.Parameters.AddWithValue("@organizer", tournament.Organisator);
            cmd.Parameters.AddWithValue("@date", tournament.Datum.Date);
            cmd.Parameters.AddWithValue("@gameFormat", tournament.Format);
            cmd.Parameters.AddWithValue("@maxTeams", tournament.MaxTeams);
            cmd.Parameters.AddWithValue("@status", tournament.Status);
            cmd.Parameters.AddWithValue("@description", (object?)tournament.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prizeMoney", (object?)tournament.PrizeMoney ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@participatingTeams", tournament.ParticipatingTeams);

            cmd.ExecuteNonQuery();
        }

        public Tournament? FindById(int id)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM tournament WHERE id = @id";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using NpgsqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapTournament(reader);
            }

            return null;
        }

        public List<Tournament> GetAll()
        {
            List<Tournament> tournaments = new List<Tournament>();

            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM tournament";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            using NpgsqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tournaments.Add(MapTournament(reader));
            }

            return tournaments;
        }

        public void Update(Tournament tournament)
        {
            using NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"UPDATE tournament SET
                name = @name,
                organizer = @organizer,
                date = @date,
                game_format = @gameFormat,
                max_teams = @maxTeams,
                status = @status,
                description = @description,
                prize_money = @prizeMoney,
                participating_teams = @participatingTeams
                WHERE id = @id";

            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", tournament.Id);
            cmd.Parameters.AddWithValue("@name", tournament.Naam);
            cmd.Parameters.AddWithValue("@organizer", tournament.Organisator);
            cmd.Parameters.AddWithValue("@date", tournament.Datum.Date);
            cmd.Parameters.AddWithValue("@gameFormat", tournament.Format);
            cmd.Parameters.AddWithValue("@maxTeams", tournament.MaxTeams);
            cmd.Parameters.AddWithValue("@status", tournament.Status);
            cmd.Parameters.AddWithValue("@description", (object?)tournament.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prizeMoney", (object?)tournament.PrizeMoney ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@participatingTeams", tournament.ParticipatingTeams);

            cmd.ExecuteNonQuery();
        }

        private Tournament MapTournament(NpgsqlDataReader reader)
        {
            return new Tournament
            {
                Id = Convert.ToInt32(reader["id"]),
                Naam = reader["name"].ToString() ?? "",
                Organisator = reader["organizer"].ToString() ?? "",
                Datum = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date")).ToDateTime(TimeOnly.MinValue),
                Format = reader["game_format"].ToString() ?? "",
                MaxTeams = Convert.ToInt32(reader["max_teams"]),
                Status = reader["status"].ToString() ?? "",
                Description = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
                PrizeMoney = reader["prize_money"] == DBNull.Value ? null : Convert.ToDecimal(reader["prize_money"]),
                ParticipatingTeams = Convert.ToInt32(reader["participating_teams"])
            };
        }
    }
}