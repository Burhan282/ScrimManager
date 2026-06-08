using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application
{
    public class TournamentService
    {
        private readonly ITournamentRepository _tournamentRepository;

        public TournamentService(ITournamentRepository tournamentRepository)
        {
            _tournamentRepository = tournamentRepository;
        }

        public void CreateTournament(Tournament tournament, DateTime date, TimeSpan time)
        {
            var combinedDateTime = date.Date + time;

            tournament.Datum = combinedDateTime;
            tournament.Status = "Open";

            _tournamentRepository.Add(tournament);
        }

        public List<Tournament> GetTournaments()
        {
            return _tournamentRepository.GetAll()
                .OrderBy(tournament => tournament.Datum)
                .ToList();
        }
        //join functionaliteit is nog niet goed
        public void JoinTournament(int tournamentId)
        {
            var tournament = _tournamentRepository.FindById(tournamentId);

            if (tournament == null)
            {
                return;
            }

            if (tournament.ParticipatingTeams >= tournament.MaxTeams)
            {
                return;
            }

            tournament.ParticipatingTeams++;

            _tournamentRepository.Update(tournament);
        }
    }
}