using ScrimManagerApplication.Application.Models;
using System.Collections.Generic;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface ITeamRepository
    {
        int Add(Team team);

        Team? FindById(int id);

        List<Team> GetAll();

        void Join(int userId, int teamId);

        List<Team> GetTeamsByUserId(int userId);

        List<User> GetTeamMembers(int teamId);

        void ApplyToTeam(int userId, int teamId);

        List<TeamJoinRequest> GetPendingRequestsForCaptain(int captainUserId);

        int? AcceptJoinRequest(int requestId, int captainUserId);

        void DeclineJoinRequest(int requestId, int captainUserId);

        void UpdateTeamRank(int teamId, Rank rank);
    }
}
