using ScrimManagerApplication.Application.Interfaces;
using ScrimManagerApplication.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrimManagerApplication.Application.Models.DTOModels;

namespace ScrimManagerApplication.Application
{
    public class TeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public void CreateTeam(CreateTeamDTO dto)
        {
            Team team = new Team();

            team.Name = dto.Name;
            team.Teamregion = dto.Teamregion;
            team.Teamrank = dto.Teamrank;

            _teamRepository.Add(team);
        }
        public List<Team> GetTeams()
        {
            return _teamRepository.GetAll();
        }
    }
}
