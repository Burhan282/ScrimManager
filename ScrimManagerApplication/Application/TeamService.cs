using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using ScrimManagerApplication.Application.Models.DTOModels;
using System.Collections.Generic;

namespace ScrimManagerApplication.Application
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public void CreateTeam(CreateTeamDTO dto, int userId)
        {
            Team team = new Team
            {
                Name = dto.Name,
                Teamregion = dto.Teamregion,
                Teamrank = dto.Teamrank,
                Description = dto.Description,
                LogoData = dto.LogoData,
                CreatedByUserId = userId
            };

            _teamRepository.Add(team);
        }

        public List<Team> GetTeams()
        {
            return _teamRepository.GetAll();
        }

        public Team? GetTeamById(int id)
        {
            return _teamRepository.FindById(id);
        }
    }
}