using ScrimManagerApplication.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrimManagerApplication.Application.Interfaces
{
    public interface ITeamRepository
    {
        void Add(Team team);
        Team? FindById(int id);
        List<Team> GetAll();
        void Join(int userId, int teamId);

    }
}
