using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
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
                new TournamentService(fakeRepository);

            Tournament tournament = new Tournament
            {
                Naam = "Rocket League Cup",
                Organisator = "Burhan",
                Format = "3v3",
                MaxTeams = 8,
                Status = "Open",
                Description = "Test tournament",
                PrizeMoney = 100,
                ParticipatingTeams = 0
            };

           
            service.CreateTournament(
                tournament,
                DateTime.Now,
                new TimeSpan(18, 0, 0));

            
            List<Tournament> tournaments =
                fakeRepository.GetAll();

            Assert.HasCount(1, tournaments);

            Assert.AreEqual(
                "Rocket League Cup",
                tournaments[0].Naam);
        }
    }
}