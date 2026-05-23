using ScrimManager.Domain;
using ScrimManager.Application.Interfaces;

namespace ScrimManager.Application
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
            // Combine date + time
            var combined = date.Date + time;

            // Zet DateTime expliciet op UTC voor EF geen problemen krijgen met tijdzones
            tournament.Datum = DateTime.SpecifyKind(combined, DateTimeKind.Utc);
            //moet nog aangepast worden feedpulse 20
            tournament.Status = "Open";

            _repository.Add(tournament);
        }

        public List<Tournament> GetTournaments()
        {
            return _repository.GetAll()
                .OrderBy(t => t.Datum)
                .ToList();
        }

        public void JoinTournament(int tournamentId) //moet nog aangepast worden. 
        {
            var tournament = _repository.FindById(tournamentId);

            if (tournament == null)
            {
                return;
            }

            if (tournament.ParticipatingTeams < tournament.MaxTeams)
            {
                tournament.ParticipatingTeams += 1;

                _repository.Update(tournament);
            }
        }
    }
}