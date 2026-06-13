using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface ITournamentRepository
    {
        void Add(Tournament tournament);

        Tournament? FindById(int id);

        List<Tournament> GetAll();

        void Update(Tournament tournament);

        void JoinTournament(int tournamentId, int? teamId, int? userId, string? entryName, List<int> playerIds);

        List<TournamentParticipationDetails> GetParticipationDetails(int tournamentId);

        List<TournamentInvitation> GetPendingInvitations(int userId);

        int GetPendingInvitationCount(int userId);

        bool UpdateInvitationStatus(int invitationId, int userId, string status);

        List<UserTournament> GetTournamentsByUserId(int userId);
    }
}
