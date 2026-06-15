using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using ScrimManagerDataAcces.DataAcces.FakeDataBases;

namespace ScrimManagerTests
{
    [TestClass]
    public class TournamentServiceTests
    {
        [TestMethod]
        public void CreateTournament_ShouldAddTournament()
        {
           
            FakeTournamentRepository fakeRepository =
                new FakeTournamentRepository();

            TournamentService service =
                new TournamentService(fakeRepository, null!);

            CreateTournamentDTO tournament = new CreateTournamentDTO
            {
                Naam = "Rocket League Cup",
                Organisator = "Burhan",
                Format = "3v3",
                MaxTeams = 8,
                Description = "Test tournament",
                PrizeMoney = 100,
                SelectedDate = DateTime.Now,
                SelectedTime = new TimeSpan(18, 0, 0)
            };

           
            service.CreateTournament(tournament);

            
            List<Tournament> tournaments =
                fakeRepository.GetAll();

            Assert.HasCount(1, tournaments);

            Assert.AreEqual(
                "Rocket League Cup",
                tournaments[0].Naam);
        }
    }
}
