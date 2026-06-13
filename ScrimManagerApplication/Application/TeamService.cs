using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScrimManagerApplication.Application
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TeamService(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }

        public void CreateTeam(CreateTeamDTO dto, int userId)
        {
            Rank averageRank = Rank.BronzeI;

            User? user = _userRepository.GetById(userId);

            if (user != null)
            {
                averageRank = user.UserRank;
            }

            Team team = new Team
            {
                Name = dto.Name,
                Teamregion = dto.Teamregion,
                Teamrank = averageRank,
                Description = dto.Description,
                LogoData = dto.LogoData,
                CreatedByUserId = userId
            };

            int teamId = _teamRepository.Add(team);

            _teamRepository.Join(userId, teamId);
        }

        public List<Team> GetTeams()
        {
            return _teamRepository.GetAll();
        }

        public Team? GetTeamById(int id)
        {
            return _teamRepository.FindById(id);
        }

        public List<Team> GetTeamsByUserId(int userId)
        {
            return _teamRepository.GetTeamsByUserId(userId);
        }

        public List<User> GetTeamMembers(int teamId)
        {
            return _teamRepository.GetTeamMembers(teamId);
        }

        public void ApplyToTeam(int userId, int teamId)
        {
            _teamRepository.ApplyToTeam(userId, teamId);
        }

        public List<TeamJoinRequest> GetPendingRequestsForCaptain(int captainUserId)
        {
            return _teamRepository.GetPendingRequestsForCaptain(captainUserId);
        }

        public void AcceptJoinRequest(int requestId, int captainUserId)
        {
            int? teamId = _teamRepository.AcceptJoinRequest(requestId, captainUserId);

            if (teamId == null)
            {
                return;
            }

            RecalculateTeamRank(teamId.Value);
        }

        public void DeclineJoinRequest(int requestId, int captainUserId)
        {
            _teamRepository.DeclineJoinRequest(requestId, captainUserId);
        }

        private void RecalculateTeamRank(int teamId)
        {
            List<User> members = _teamRepository.GetTeamMembers(teamId);

            if (members == null || !members.Any())
            {
                return;
            }

            double averageRankNumber = members.Average(user => (int)user.UserRank);

            int roundedRankNumber = (int)Math.Round(averageRankNumber);

            Rank newTeamRank = (Rank)roundedRankNumber;

            _teamRepository.UpdateTeamRank(teamId, newTeamRank);
        }
    }
}
