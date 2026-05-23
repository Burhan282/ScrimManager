using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerApplication.Application
{
    public class TournamentService
    {
        private readonly ITournamentRepository _repository;

        public TournamentService(ITournamentRepository repository)
        {
            _repository = repository;
        }

        public void CreateTournament(Tournament tournament, DateTime date, TimeSpan time)
        {
            var combinedDateTime = date.Date + time;

            tournament.Datum = combinedDateTime;
            tournament.Status = "Open";

            _repository.Add(tournament);
        }

        public List<Tournament> GetTournaments()
        {
            return _repository.GetAll()
                .OrderBy(tournament => tournament.Datum)
                .ToList();
        }

        public void JoinTournament(int tournamentId)
        {
            var tournament = _repository.FindById(tournamentId);

            if (tournament == null)
            {
                return;
            }

            if (tournament.ParticipatingTeams >= tournament.MaxTeams)
            {
                return;
            }

            tournament.ParticipatingTeams++;

            _repository.Update(tournament);
        }
    }
}