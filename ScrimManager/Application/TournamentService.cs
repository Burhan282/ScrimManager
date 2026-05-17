using ScrimManager.Domain;
using ScrimManager.DataAccess;

namespace ScrimManager.Application
{
    public class TournamentService
    {
        private readonly TournamentRepository _repository;

        public TournamentService(TournamentRepository repository)
        {
            _repository = repository;
        }

        public void CreateTournament(Tournament tournament, DateTime date, TimeSpan time)
        {
            // Combine date + time
            var combined = date.Date + time;

            // Zet DateTime expliciet op UTC
            tournament.Datum = DateTime.SpecifyKind(combined, DateTimeKind.Utc);

            tournament.Status = "Open";

            _repository.Add(tournament);
        }
    }
}