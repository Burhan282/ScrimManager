using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScrimManagerDataAcces.DataAcces.FakeDataBases
{
    public class FakeTeamRepository : ITeamRepository
    {
        private readonly List<Team> teams = new();
        private readonly List<(int UserId, int TeamId)> teamMembers = new();
        private readonly List<TeamJoinRequest> teamJoinRequests = new();
        private readonly IUserRepository? userRepository;

        public FakeTeamRepository()
        {
        }

        public FakeTeamRepository(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public int Add(Team team)
        {
            team.Id = teams.Count + 1;
            teams.Add(team);

            return team.Id;
        }

        public Team? FindById(int id)
        {
            return teams.FirstOrDefault(team => team.Id == id);
        }

        public List<Team> GetAll()
        {
            return teams;
        }

        public void Join(int userId, int teamId)
        {
            teamMembers.Add((userId, teamId));
        }

        public List<Team> GetTeamsByUserId(int userId)
        {
            List<int> teamIds = teamMembers
                .Where(member => member.UserId == userId)
                .Select(member => member.TeamId)
                .ToList();

            return teams
                .Where(team => teamIds.Contains(team.Id))
                .ToList();
        }

        public List<User> GetTeamMembers(int teamId)
        {
            if (userRepository == null)
            {
                return new List<User>();
            }

            return teamMembers
                .Where(member => member.TeamId == teamId)
                .Select(member => userRepository.GetById(member.UserId))
                .Where(user => user != null)
                .Select(user => user!)
                .ToList();
        }

        public void ApplyToTeam(int userId, int teamId)
        {
            teamJoinRequests.Add(new TeamJoinRequest
            {
                Id = teamJoinRequests.Count + 1,
                UserId = userId,
                TeamId = teamId,
                Status = "Pending"
            });
        }

        public List<TeamJoinRequest> GetPendingRequestsForCaptain(int captainUserId)
        {
            return teamJoinRequests
                .Where(request => request.Status == "Pending")
                .ToList();
        }

        public int? AcceptJoinRequest(int requestId, int captainUserId)
        {
            TeamJoinRequest? request = teamJoinRequests
                .FirstOrDefault(request => request.Id == requestId);

            if (request == null)
            {
                return null;
            }

            request.Status = "Accepted";
            Join(request.UserId, request.TeamId);

            return request.TeamId;
        }

        public void DeclineJoinRequest(int requestId, int captainUserId)
        {
            TeamJoinRequest? request = teamJoinRequests
                .FirstOrDefault(request => request.Id == requestId);

            if (request != null)
            {
                request.Status = "Declined";
            }
        }

        public void UpdateTeamRank(int teamId, Rank rank)
        {
            Team? team = FindById(teamId);

            if (team != null)
            {
                team.Teamrank = rank;
            }
        }
    }
}
