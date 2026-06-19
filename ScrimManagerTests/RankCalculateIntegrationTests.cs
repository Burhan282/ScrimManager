using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using ScrimManagerDataAcces.DataAcces.FakeDataBases;

namespace ScrimManagerTests
{
    [TestClass]
    public class RankCalculateIntegrationTests
    {
        [TestMethod]
        public void AcceptJoinRequest_ShouldRecalculateTeamRank()
        {
            // Arrange
            FakeUserRepository fakeUserRepository = new FakeUserRepository();
            FakeTeamRepository fakeTeamRepository = new FakeTeamRepository(fakeUserRepository);

            fakeUserRepository.Add(new User
            {
                Id = 1,
                Username = "Captain",
                UserRank = Rank.GoldI,
                UserRegion = Region.EU
            });

            fakeUserRepository.Add(new User
            {
                Id = 2,
                Username = "Player",
                UserRank = Rank.GoldIII,
                UserRegion = Region.EU
            });

            TeamService teamService = new TeamService(fakeTeamRepository, fakeUserRepository);

            CreateTeamDTO team = new CreateTeamDTO
            {
                Name = "Team Alpha",
                Teamregion = Region.EU
            };

            teamService.CreateTeam(team, 1);
            teamService.ApplyToTeam(2, 1);

            // Act
            teamService.AcceptJoinRequest(1, 1);

            // Assert
            Team? updatedTeam = teamService.GetTeamById(1);

            Assert.IsNotNull(updatedTeam);
            Assert.AreEqual(Rank.GoldII, updatedTeam.Teamrank);
        }
    }
}
