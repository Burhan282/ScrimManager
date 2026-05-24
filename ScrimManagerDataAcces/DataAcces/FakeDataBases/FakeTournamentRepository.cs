using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;

namespace ScrimManagerDataAcces.DataAcces.FakeDataBases
{
    public class FakeTournamentRepository : ITournamentRepository
    {
        private List<Tournament> tournaments = new();

        public void Add(Tournament tournament)
        {
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

        public void Update(Tournament tournament)
        {
        }
    }
}