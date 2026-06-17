using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using ScrimManagerDataAcces.DataAcces.FakeDataBases;

namespace ScrimManagerTests
{
    [TestClass]
    public class TeamServiceTests
    {
        [TestMethod]
        public void AddTeam_ShouldAddTeam()
        {
            // Arrange
            FakeTeamRepository fakeTeamRepository = new FakeTeamRepository();
            FakeUserRepository fakeUserRepository = new FakeUserRepository();

            fakeUserRepository.Add(new User
            {
                Id = 1,
                Username = "Burhan",
                UserRank = Rank.GoldIII,
                UserRegion = Region.EU
            });

            TeamService teamService = new TeamService(fakeTeamRepository, fakeUserRepository);

            CreateTeamDTO team = new CreateTeamDTO
            {
                Name = "Team Alpha",
                Teamregion = Region.EU,
                Description = "Test team"
            };

            // Act
            teamService.CreateTeam(team, 1);

            // Assert
            List<Team> teams = teamService.GetTeams();

            Assert.HasCount(1, teams);
            Assert.AreEqual("Team Alpha", teams[0].Name);
            Assert.AreEqual(1, teams[0].CreatedByUserId);
            Assert.AreEqual(Rank.GoldIII, teams[0].Teamrank);
        }

        [TestMethod]
        public void JoinTeam_ShouldAddCreatorToTeam()
        {
            // Arrange
            FakeTeamRepository fakeTeamRepository = new FakeTeamRepository();
            FakeUserRepository fakeUserRepository = new FakeUserRepository();

            fakeUserRepository.Add(new User
            {
                Id = 1,
                Username = "Burhan",
                UserRank = Rank.GoldIII,
                UserRegion = Region.EU
            });

            TeamService teamService = new TeamService(fakeTeamRepository, fakeUserRepository);

            CreateTeamDTO team = new CreateTeamDTO
            {
                Name = "Team Alpha",
                Teamregion = Region.EU
            };

            // Act
            teamService.CreateTeam(team, 1);

            // Assert
            List<Team> teamsFromUser = teamService.GetTeamsByUserId(1);

            Assert.HasCount(1, teamsFromUser);
            Assert.AreEqual("Team Alpha", teamsFromUser[0].Name);
        }

    }
}
