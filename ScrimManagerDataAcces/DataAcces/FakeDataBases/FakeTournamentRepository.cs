using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScrimManagerDataAcces.DataAcces.FakeDataBases
{
    public class FakeTournamentRepository : ITournamentRepository
    {
        private readonly List<Tournament> tournaments = new();

        public void Add(Tournament tournament)
        {
            if (tournament.Id == 0)
            {
                tournament.Id = tournaments.Count + 1;
            }

            tournaments.Add(tournament);
        }

        public Tournament? FindById(int id)
        {
            return tournaments.FirstOrDefault(t => t.Id == id);
        }

        public List<Tournament> GetAll()
        {
            return tournaments;
        }

        public List<TournamentParticipationDetails> GetParticipationDetails(int tournamentId)
        {
            return new List<TournamentParticipationDetails>();
        }

        public List<TournamentInvitation> GetPendingInvitations(int userId)
        {
            return new List<TournamentInvitation>();
        }

        public int GetPendingInvitationCount(int userId)
        {
            return 0;
        }

        public bool UpdateInvitationStatus(int invitationId, int userId, string status)
        {
            return false;
        }

        public List<UserTournament> GetTournamentsByUserId(int userId)
        {
            return new List<UserTournament>();
        }

        public void Update(Tournament tournament)
        {
            var existing = tournaments.FirstOrDefault(t => t.Id == tournament.Id);

            if (existing == null)
                return;

            existing.ParticipatingTeams = tournament.ParticipatingTeams;
        }

        public void JoinTournament(
            int tournamentId,
            int? teamId,
            int? userId,
            string? entryName,
            List<int> playerIds)
        {
        }
    }
}
