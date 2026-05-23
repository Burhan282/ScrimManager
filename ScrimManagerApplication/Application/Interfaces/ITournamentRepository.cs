using ScrimManager.Domain;

namespace ScrimManager.Application.Interfaces
{
    public interface ITournamentRepository
    {
        void Add(Tournament tournament);

        Tournament? FindById(int id);

        List<Tournament> GetAll();

        void Update(Tournament tournament);
    }
}