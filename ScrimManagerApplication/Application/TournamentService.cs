using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;

namespace ScrimManagerApplication.Application
{
    public class TournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamRepository _teamRepository;

        public TournamentService(
            ITournamentRepository tournamentRepository,
            ITeamRepository teamRepository)
        {
            _tournamentRepository = tournamentRepository;
            _teamRepository = teamRepository;
        }

        public void CreateTournament(CreateTournamentDTO dto)
        {
            if (!dto.SelectedDate.HasValue || !dto.SelectedTime.HasValue)
                throw new ArgumentException("A tournament date and time are required.");

            var tournament = new Tournament
            {
                Naam = dto.Naam,
                Organisator = dto.Organisator,
                Datum = dto.SelectedDate.Value.Date + dto.SelectedTime.Value,
                Format = dto.Format,
                MaxTeams = dto.MaxTeams,
                Status = "Open",
                Description = dto.Description,
                PrizeMoney = dto.PrizeMoney,
                ParticipatingTeams = 0
            };

            _tournamentRepository.Add(tournament);
        }

        public List<Tournament> GetTournaments()
        {
            return _tournamentRepository.GetAll();
        }

        public Tournament? GetTournamentById(int id)
        {
            return _tournamentRepository.FindById(id);
        }

        public List<TournamentParticipationDetails> GetParticipationDetails(int tournamentId)
        {
            return _tournamentRepository.GetParticipationDetails(tournamentId);
        }

        public List<TournamentInvitation> GetPendingInvitations(int userId)
        {
            return _tournamentRepository.GetPendingInvitations(userId);
        }

        public int GetPendingInvitationCount(int userId)
        {
            return _tournamentRepository.GetPendingInvitationCount(userId);
        }

        public bool AcceptInvitation(int invitationId, int userId)
        {
            return _tournamentRepository.UpdateInvitationStatus(invitationId, userId, "Accepted");
        }

        public bool DeclineInvitation(int invitationId, int userId)
        {
            return _tournamentRepository.UpdateInvitationStatus(invitationId, userId, "Declined");
        }

        public List<UserTournament> GetTournamentsByUserId(int userId)
        {
            return _tournamentRepository.GetTournamentsByUserId(userId);
        }

        public string? JoinTournament(JoinTournamentRequestDTO request, int userId)
        {
            var tournament = _tournamentRepository.FindById(request.TournamentId);

            if (tournament == null)
                return "Tournament not found.";

            if (tournament.IsFull)
                return "This tournament is full.";

            if (tournament.IsSolo)
            {
                request.TeamId = null;
                request.PlayerIds = new List<int> { userId };
            }
            else
            {
                var selectedTeam = _teamRepository.FindById(request.TeamId ?? 0);

                if (selectedTeam == null || selectedTeam.CreatedByUserId != userId)
                    return "Select a team that you created.";

                var selectedPlayerIds = request.PlayerIds.Distinct().ToList();
                var memberIds = _teamRepository.GetTeamMembers(selectedTeam.Id)
                    .Select(member => member.Id)
                    .ToHashSet();

                if (selectedPlayerIds.Count != tournament.RequiredPlayers)
                    return $"Select exactly {tournament.RequiredPlayers} players.";

                if (selectedPlayerIds.Any(playerId => !memberIds.Contains(playerId)))
                    return "Every selected player must belong to the selected team.";

                request.PlayerIds = selectedPlayerIds;
            }

            _tournamentRepository.JoinTournament(
                request.TournamentId,
                request.TeamId,
                userId,
                request.EntryName,
                request.PlayerIds
            );

            tournament.ParticipatingTeams++;

            _tournamentRepository.Update(tournament);
            return null;
        }
    }
}
