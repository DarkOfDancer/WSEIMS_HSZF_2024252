using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Application;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252.Tests
{
    [TestFixture]
    public class TeamServiceTests
    {
        private Mock<FormulaOneDbContext> _dbContextMock;
        private Mock<DbSet<TeamEntity>> _teamsDbSetMock;
        private List<TeamEntity> _fakeTeams;
        private TeamService _teamService;

        [SetUp]
        public void SetUp()
        {
            _fakeTeams = new List<TeamEntity>
            {
                new TeamEntity { Id = "1", teamName = "Red Bull", year = 2023, teamPrincipal = "Christian Horner", headquarters = "Milton Keynes", constructorsChampionshipWins = 6 },
                new TeamEntity { Id = "2", teamName = "Mercedes", year = 2023, teamPrincipal = "Toto Wolff", headquarters = "Brackley", constructorsChampionshipWins = 8 }
            };

            _teamsDbSetMock = new Mock<DbSet<TeamEntity>>();
            var queryable = _fakeTeams.AsQueryable();

            _teamsDbSetMock.As<IQueryable<TeamEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _teamsDbSetMock.As<IQueryable<TeamEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _teamsDbSetMock.As<IQueryable<TeamEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _teamsDbSetMock.As<IQueryable<TeamEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            _dbContextMock = new Mock<FormulaOneDbContext>();
            _dbContextMock.Setup(c => c.Teams).Returns(_teamsDbSetMock.Object);

            _teamService = new TeamService(_dbContextMock.Object);
        }

        [Test]
        public void GetById_ReturnsCorrectTeam_WhenIdExists()
        {
            var result = _teamService.GetById("1");

            Assert.IsNotNull(result);
            Assert.AreEqual("Red Bull", result.teamName);
        }

        [Test]
        public void GetById_ReturnsNull_WhenIdDoesNotExist()
        {
            var result = _teamService.GetById("999");

            Assert.IsNull(result);
        }

        [Test]
        public void GetTeamsPaged_ReturnsCorrectNumberOfTeams()
        {
            var result = _teamService.GetTeamsPaged(1, 1);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Search_ByNameExactMatch_ReturnsCorrectTeam()
        {
            var result = _teamService.Search("name", "Red Bull", "e");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Red Bull", result[0].teamName);
        }

        [Test]
        public void Search_ByPrincipalPartialMatch_ReturnsMatchingTeams()
        {
            var result = _teamService.Search("principal", "wolff", "c");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mercedes", result[0].teamName);
        }
    }
}
