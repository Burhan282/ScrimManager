using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;

namespace ScrimManagerApplication.Application
{
    public class TournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
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

        public bool JoinTournament(
            int tournamentId,
            int? teamId,
            int? userId,
            string? entryName,
            List<int>? playerIds = null)
        {
            var tournament = _tournamentRepository.FindById(tournamentId);

            if (tournament == null)
                return false;

            if (tournament.ParticipatingTeams >= tournament.MaxTeams)
                return false;

            _tournamentRepository.JoinTournament(
                tournamentId,
                teamId,
                userId,
                entryName,
                playerIds ?? new List<int>()
            );

            tournament.ParticipatingTeams++;

            _tournamentRepository.Update(tournament);
            return true;
        }
    }
}
