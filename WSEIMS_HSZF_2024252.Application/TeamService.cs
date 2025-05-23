using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;
using System.Globalization;
using WSEIMS_HSZF_2024252.Application;

namespace WSEIMS_HSZF_2024252.Application
{

        public interface ITeamService
        {
            List<TeamEntity> GetTeamsPaged(int page, int size);
            List<TeamEntity> Search(string field, string value, string searchType);
            bool Delete(string id);
            TeamEntity GetById(string id);
            bool Update(TeamEntity team);
            List<TeamEntity> ImportFromDirectory(string path);
        }
    public class TeamService : ITeamService
    {
        private readonly ITeamDataProvider _context;

        public TeamService(ITeamDataProvider context)
        {
            _context = context;
        }

       public List<TeamEntity> GetTeamsPaged(int page, int size)
        {
            return _context.Context().Teams
                .OrderBy(t => t.year)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }
        public List<TeamEntity> Search(string field, string value, string searchType)
        {
           return _context.Search(field, value, searchType);
        }

        public bool Delete(string id)
        {
            return _context.Delete(id);
        }

        public TeamEntity GetById(string id)
        {
            return _context.GetById(id);
        }

        public bool Update(TeamEntity team)
        {
            return _context.Update(team);
        }

        public List<TeamEntity> ImportFromDirectory(string path)
        {
            var importer = new JsonImporter();
            return importer.ImportTeamsFromNEWDirectory(path);
        }


    }

}
