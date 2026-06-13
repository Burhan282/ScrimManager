using Npgsql;
using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System;
using System.Collections.Generic;

namespace ScrimManagerDataAccess
{
    public class TournamentRepository : ITournamentRepository
    {
        private static readonly object InvitationTableLock = new();
        private static bool invitationTableReady;
        private readonly string connectionString;

        public TournamentRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(Tournament tournament)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                INSERT INTO tournament
                (name, organizer, date, game_format, max_teams, status, description, prize_money, participating_teams)
                VALUES
                (@name, @organizer, @date, @gameFormat, @maxTeams, @status, @description, @prizeMoney, 0);
            ";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@name", tournament.Naam);
            cmd.Parameters.AddWithValue("@organizer", tournament.Organisator);
            cmd.Parameters.AddWithValue("@date", tournament.Datum);
            cmd.Parameters.AddWithValue("@gameFormat", tournament.Format);
            cmd.Parameters.AddWithValue("@maxTeams", tournament.MaxTeams);
            cmd.Parameters.AddWithValue("@status", tournament.Status);
            cmd.Parameters.AddWithValue("@description", (object?)tournament.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prizeMoney", (object?)tournament.PrizeMoney ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public List<Tournament> GetAll()
        {
            var list = new List<Tournament>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM tournament ORDER BY date";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Tournament
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Naam = reader["name"].ToString() ?? "",
                    Organisator = reader["organizer"].ToString() ?? "",
                    Datum = Convert.ToDateTime(reader["date"]),
                    Format = reader["game_format"].ToString() ?? "",
                    MaxTeams = Convert.ToInt32(reader["max_teams"]),
                    Status = reader["status"].ToString() ?? "",
                    ParticipatingTeams = Convert.ToInt32(reader["participating_teams"]),
                    Description = reader["description"] == DBNull.Value
                        ? null
                        : reader["description"].ToString(),
                    PrizeMoney = reader["prize_money"] == DBNull.Value
                        ? null
                        : Convert.ToDecimal(reader["prize_money"])
                });
            }

            return list;
        }

        public Tournament? FindById(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT * FROM tournament WHERE id = @id";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Tournament
            {
                Id = Convert.ToInt32(reader["id"]),
                Naam = reader["name"].ToString() ?? "",
                Organisator = reader["organizer"].ToString() ?? "",
                Datum = Convert.ToDateTime(reader["date"]),
                Format = reader["game_format"].ToString() ?? "",
                MaxTeams = Convert.ToInt32(reader["max_teams"]),
                Status = reader["status"].ToString() ?? "",
                ParticipatingTeams = Convert.ToInt32(reader["participating_teams"]),
                Description = reader["description"] == DBNull.Value ? null : reader["description"].ToString(),
                PrizeMoney = reader["prize_money"] == DBNull.Value
                    ? null
                    : Convert.ToDecimal(reader["prize_money"])
            };
        }

        public List<TournamentParticipationDetails> GetParticipationDetails(int tournamentId)
        {
            var participations = new Dictionary<int, TournamentParticipationDetails>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            const string query = @"
                SELECT
                    tp.id AS participant_id,
                    tp.team_id,
                    tp.entry_name,
                    participant_user.username AS participant_username,
                    team.name AS team_name,
                    team.logo_data AS team_logo_data,
                    selected_player.id AS player_id,
                    selected_player.username AS player_username
                FROM tournament_participants tp
                LEFT JOIN team
                    ON team.id = tp.team_id
                LEFT JOIN ""user"" participant_user
                    ON participant_user.id = tp.user_id
                LEFT JOIN tournament_participant_players tpp
                    ON tpp.participant_id = tp.id
                LEFT JOIN tournament_player_invitation invitation
                    ON invitation.participant_id = tp.id
                    AND invitation.user_id = tpp.user_id
                    AND invitation.status = 'Accepted'
                LEFT JOIN ""user"" selected_player
                    ON selected_player.id = tpp.user_id
                    AND invitation.id IS NOT NULL
                WHERE tp.tournament_id = @tournamentId
                ORDER BY tp.id, selected_player.username;
            ";

            EnsureInvitationTable(conn);

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@tournamentId", tournamentId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int participantId = Convert.ToInt32(reader["participant_id"]);

                if (!participations.TryGetValue(participantId, out var participation))
                {
                    string entryName = reader["entry_name"] == DBNull.Value
                        ? string.Empty
                        : reader["entry_name"].ToString() ?? string.Empty;
                    string teamName = reader["team_name"] == DBNull.Value
                        ? string.Empty
                        : reader["team_name"].ToString() ?? string.Empty;
                    string username = reader["participant_username"] == DBNull.Value
                        ? string.Empty
                        : reader["participant_username"].ToString() ?? string.Empty;

                    participation = new TournamentParticipationDetails
                    {
                        ParticipantId = participantId,
                        TeamId = reader["team_id"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["team_id"]),
                        TeamLogoData = reader["team_logo_data"] == DBNull.Value
                            ? null
                            : (byte[])reader["team_logo_data"],
                        DisplayName = !string.IsNullOrWhiteSpace(teamName)
                            ? teamName
                            : !string.IsNullOrWhiteSpace(entryName)
                                ? entryName
                                : username
                    };

                    participations.Add(participantId, participation);
                }

                if (reader["player_id"] != DBNull.Value)
                {
                    participation.Players.Add(new TournamentParticipationPlayer
                    {
                        UserId = Convert.ToInt32(reader["player_id"]),
                        Username = reader["player_username"].ToString() ?? string.Empty
                    });
                }
            }

            return participations.Values.ToList();
        }

        public void Update(Tournament tournament)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
                UPDATE tournament SET
                    participating_teams = @participatingTeams
                WHERE id = @id
            ";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", tournament.Id);
            cmd.Parameters.AddWithValue("@participatingTeams", tournament.ParticipatingTeams);

            cmd.ExecuteNonQuery();
        }

        public void JoinTournament(
            int tournamentId,
            int? teamId,
            int? userId,
            string? entryName,
            List<int> playerIds)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            EnsureInvitationTable(conn);
            using var transaction = conn.BeginTransaction();

            string query = @"
                INSERT INTO tournament_participants
                (tournament_id, team_id, user_id, entry_name)
                VALUES
                (@tournamentId, @teamId, @userId, @entryName)
                RETURNING id;
            ";

            using var cmd = new NpgsqlCommand(query, conn, transaction);

            cmd.Parameters.AddWithValue("@tournamentId", tournamentId);
            cmd.Parameters.AddWithValue("@teamId", (object?)teamId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@userId", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@entryName", (object?)entryName ?? DBNull.Value);

            int participantId = Convert.ToInt32(cmd.ExecuteScalar());

            foreach (int playerId in playerIds.Distinct())
            {
                const string playerQuery = @"
                    INSERT INTO tournament_participant_players
                    (participant_id, user_id)
                    VALUES
                    (@participantId, @userId);
                ";

                using var playerCmd = new NpgsqlCommand(playerQuery, conn, transaction);
                playerCmd.Parameters.AddWithValue("@participantId", participantId);
                playerCmd.Parameters.AddWithValue("@userId", playerId);
                playerCmd.ExecuteNonQuery();

                const string invitationQuery = @"
                    INSERT INTO tournament_player_invitation
                    (participant_id, user_id, invited_by_user_id, status)
                    VALUES
                    (@participantId, @playerId, @invitedByUserId, @status)
                    ON CONFLICT (participant_id, user_id) DO NOTHING;
                ";

                using var invitationCmd = new NpgsqlCommand(invitationQuery, conn, transaction);
                invitationCmd.Parameters.AddWithValue("@participantId", participantId);
                invitationCmd.Parameters.AddWithValue("@playerId", playerId);
                invitationCmd.Parameters.AddWithValue("@invitedByUserId", (object?)userId ?? DBNull.Value);
                invitationCmd.Parameters.AddWithValue(
                    "@status",
                    userId.HasValue && userId.Value == playerId ? "Accepted" : "Pending");
                invitationCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public List<TournamentInvitation> GetPendingInvitations(int userId)
        {
            var invitations = new List<TournamentInvitation>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            EnsureInvitationTable(conn);

            const string query = @"
                SELECT
                    invitation.id,
                    invitation.participant_id,
                    invitation.user_id,
                    invitation.status,
                    tournament.id AS tournament_id,
                    tournament.name AS tournament_name,
                    tournament.game_format,
                    tournament.date,
                    team.name AS team_name
                FROM tournament_player_invitation invitation
                INNER JOIN tournament_participants participant
                    ON participant.id = invitation.participant_id
                INNER JOIN tournament
                    ON tournament.id = participant.tournament_id
                LEFT JOIN team
                    ON team.id = participant.team_id
                WHERE invitation.user_id = @userId
                  AND invitation.status = 'Pending'
                ORDER BY tournament.date, invitation.created_at DESC;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                invitations.Add(new TournamentInvitation
                {
                    Id = Convert.ToInt32(reader["id"]),
                    ParticipantId = Convert.ToInt32(reader["participant_id"]),
                    TournamentId = Convert.ToInt32(reader["tournament_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    TournamentName = reader["tournament_name"].ToString() ?? string.Empty,
                    TeamName = reader["team_name"] == DBNull.Value
                        ? string.Empty
                        : reader["team_name"].ToString() ?? string.Empty,
                    Format = reader["game_format"].ToString() ?? string.Empty,
                    StartDate = Convert.ToDateTime(reader["date"]),
                    Status = reader["status"].ToString() ?? "Pending"
                });
            }

            return invitations;
        }

        public int GetPendingInvitationCount(int userId)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            EnsureInvitationTable(conn);

            const string query = @"
                SELECT COUNT(*)
                FROM tournament_player_invitation
                WHERE user_id = @userId
                  AND status = 'Pending';
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateInvitationStatus(int invitationId, int userId, string status)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            EnsureInvitationTable(conn);

            const string query = @"
                UPDATE tournament_player_invitation
                SET status = @status
                WHERE id = @invitationId
                  AND user_id = @userId
                  AND status = 'Pending';
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@invitationId", invitationId);
            cmd.Parameters.AddWithValue("@userId", userId);
            return cmd.ExecuteNonQuery() == 1;
        }

        public List<UserTournament> GetTournamentsByUserId(int userId)
        {
            var tournaments = new List<UserTournament>();

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            EnsureInvitationTable(conn);

            const string query = @"
                SELECT DISTINCT
                    tournament.id,
                    tournament.name,
                    tournament.game_format,
                    tournament.date,
                    tournament.status,
                    tournament.participating_teams,
                    tournament.max_teams,
                    team.name AS team_name
                FROM tournament
                INNER JOIN tournament_participants participant
                    ON participant.tournament_id = tournament.id
                LEFT JOIN team
                    ON team.id = participant.team_id
                LEFT JOIN tournament_player_invitation invitation
                    ON invitation.participant_id = participant.id
                    AND invitation.user_id = @userId
                    AND invitation.status = 'Accepted'
                WHERE (
                    participant.user_id = @userId
                    AND participant.team_id IS NULL
                )
                OR invitation.id IS NOT NULL
                ORDER BY tournament.date;
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tournaments.Add(new UserTournament
                {
                    TournamentId = Convert.ToInt32(reader["id"]),
                    TournamentName = reader["name"].ToString() ?? string.Empty,
                    TeamName = reader["team_name"] == DBNull.Value
                        ? string.Empty
                        : reader["team_name"].ToString() ?? string.Empty,
                    Format = reader["game_format"].ToString() ?? string.Empty,
                    StartDate = Convert.ToDateTime(reader["date"]),
                    TournamentStatus = reader["status"].ToString() ?? string.Empty,
                    ParticipatingTeams = Convert.ToInt32(reader["participating_teams"]),
                    MaxTeams = Convert.ToInt32(reader["max_teams"])
                });
            }

            return tournaments;
        }

        private static void EnsureInvitationTable(NpgsqlConnection conn)
        {
            if (invitationTableReady)
                return;

            lock (InvitationTableLock)
            {
                if (invitationTableReady)
                    return;

                const string createQuery = @"
                    CREATE TABLE IF NOT EXISTS tournament_player_invitation
                    (
                        id SERIAL PRIMARY KEY,
                        participant_id INTEGER NOT NULL,
                        user_id INTEGER NOT NULL,
                        invited_by_user_id INTEGER NULL,
                        status VARCHAR(20) NOT NULL DEFAULT 'Pending',
                        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
                    );

                    CREATE UNIQUE INDEX IF NOT EXISTS
                        ux_tournament_player_invitation_participant_user
                    ON tournament_player_invitation (participant_id, user_id);

                    INSERT INTO tournament_participant_players
                        (participant_id, user_id)
                    SELECT
                        participant.id,
                        participant.user_id
                    FROM tournament_participants participant
                    INNER JOIN tournament
                        ON tournament.id = participant.tournament_id
                    WHERE tournament.game_format = '1v1'
                      AND participant.user_id IS NOT NULL
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM tournament_participant_players player
                          WHERE player.participant_id = participant.id
                            AND player.user_id = participant.user_id
                      );

                    INSERT INTO tournament_player_invitation
                        (participant_id, user_id, invited_by_user_id, status)
                    SELECT
                        player.participant_id,
                        player.user_id,
                        participant.user_id,
                        CASE
                            WHEN player.user_id = participant.user_id THEN 'Accepted'
                            ELSE 'Pending'
                        END
                    FROM tournament_participant_players player
                    INNER JOIN tournament_participants participant
                        ON participant.id = player.participant_id
                    ON CONFLICT (participant_id, user_id) DO NOTHING;
                ";

                using var cmd = new NpgsqlCommand(createQuery, conn);
                cmd.ExecuteNonQuery();
                invitationTableReady = true;
            }
        }
    }
}
