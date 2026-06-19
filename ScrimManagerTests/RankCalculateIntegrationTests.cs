using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using ScrimManagerApplication.Application;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using ScrimManagerDataAcces.DataAcces.FakeDataBases;
using ScrimManagerPresentation.Pages.Presentation.Teams;

namespace ScrimManagerTests
{
    [TestClass]
    public class RankCalculateIntegrationTests
    {
        [TestMethod]
        public async Task AcceptJoinRequest_WithPresentationApplicationAndDataLayer_ShouldRecalculateTeamRank()
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

            CreateTeamModel createTeamPage = new CreateTeamModel(teamService)
            {
                CreateTeamDTO = new CreateTeamDTO
                {
                    Name = "Team Alpha",
                    Teamregion = Region.EU
                }
            };
            SetPageContext(createTeamPage, userId: 1);

            TeamDetailsModel teamDetailsPage = new TeamDetailsModel(teamService);
            SetPageContext(teamDetailsPage, userId: 2);

            TeamRequestModel teamRequestPage = new TeamRequestModel(teamService);
            SetPageContext(teamRequestPage, userId: 1);

            // Act
            await createTeamPage.OnPostAsync();
            teamDetailsPage.OnPostApply(1);
            teamRequestPage.OnPostAccept(1);

            // Assert
            Team? updatedTeam = teamService.GetTeamById(1);

            Assert.IsNotNull(updatedTeam);
            Assert.AreEqual(Rank.GoldII, updatedTeam.Teamrank);
        }

        private static void SetPageContext(PageModel pageModel, int userId)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext
            {
                Session = new TestSession()
            };
            httpContext.Session.SetInt32("UserId", userId);

            pageModel.PageContext = new PageContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData()
            };
            pageModel.TempData = new TempDataDictionary(httpContext, new TestTempDataProvider());
        }

        private class TestTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context)
            {
                return new Dictionary<string, object>();
            }

            public void SaveTempData(HttpContext context, IDictionary<string, object> values)
            {
            }
        }

        private class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> values = new();

            public IEnumerable<string> Keys => values.Keys;

            public string Id { get; } = Guid.NewGuid().ToString();

            public bool IsAvailable => true;

            public void Clear()
            {
                values.Clear();
            }

            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task LoadAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public void Remove(string key)
            {
                values.Remove(key);
            }

            public void Set(string key, byte[] value)
            {
                values[key] = value;
            }

            public bool TryGetValue(string key, out byte[] value)
            {
                if (values.TryGetValue(key, out byte[]? storedValue))
                {
                    value = storedValue;
                    return true;
                }

                value = Array.Empty<byte>();
                return false;
            }
        }
    }
}
