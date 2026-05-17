using ScrimManager.Data;
using ScrimManager.Domain;

namespace ScrimManager.DataAccess
{
    public class TournamentRepository
    {
        private readonly ScrimManagerDbContext _db;

        public TournamentRepository(ScrimManagerDbContext db)
        {
            _db = db;
        }

        public void Add(Tournament tournament)
        {
            _db.Tournaments.Add(tournament);
            _db.SaveChanges();
        }

        public Tournament FindById(int id)
        {
            return _db.Tournaments.FirstOrDefault(t => t.Id == id);
        }

        public List<Tournament> GetAll()
        {
            return _db.Tournaments.ToList();
        }
    }
}