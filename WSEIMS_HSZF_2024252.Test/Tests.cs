using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Application;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;
using Moq;


namespace WSEIMS_HSZF_2024252.Tests
{
    [TestFixture]
    public class TeamServiceTests
    {
        private Mock<ITeamDataProvider> _teamServiceMock;

        private TeamService _teamService;

        [SetUp]
        public void Setup()
        {
            _teamServiceMock = new Mock<ITeamDataProvider>();
            _teamService = new TeamService(_teamServiceMock.Object);
        }

        [Test]
        public void GetTeamById_ReturnsTeam_WhenTeamExists()
        {
            var teamId = "1";
            var expectedTEAM = new TeamEntity { Id = teamId, teamName = "TEST TEAM" };

            _teamServiceMock.Setup(p => p.GetById(teamId)).Returns(expectedTEAM);

            var result = _teamService.GetById(teamId);

            Assert.That(result != null);
            Assert.That(expectedTEAM.Id == result.Id);
        }

        [Test]
        public void GetTeamById_ReturnsNull_WhenTeamDoesNotExists()
        {
            var teamID = "93asd";
            _teamServiceMock.Setup(p => p.GetById(teamID)).Returns((TeamEntity)null);
            var result = _teamService.GetById(teamID);
            Assert.That(result == null);
        }
        [Test]
        public void SearchTeam_ReturnList_WhenTeamExist_Contains()
        {
            var field ="name";
            var value = "Ferrari";
            var searchType = "c";

            List<TeamEntity> expected = new List<TeamEntity> { new TeamEntity { teamName = "Scuda Ferrari" }, new TeamEntity { teamName="Scuda Ferrari"} };

            _teamServiceMock.Setup(p => p.Search(field, value, searchType)).Returns(expected);

            var result = _teamService.Search(field, value, searchType);

            Assert.That(result == expected);
        }
        [Test]
        public void SearchTeam_ReturnNull_WhenTeamDoesNotExist_Contains()
        {
            var field = "name";
            var value = "Ferrari";
            var searchType = "c";

            _teamServiceMock.Setup(p => p.Search(field, value, searchType)).Returns((List<TeamEntity>)null);
            var result = _teamService.Search(field, value, searchType);
            Assert.That(result == null);
        }
        [Test]
        public void SearchTeam_ReturnList_WhenTeamExist_Equals()
        {
            var field = "name";
            var value = "AMG Mercedes";
            var searchType = "e";

            List<TeamEntity> expected = new List<TeamEntity> { new TeamEntity { teamName = "Scuda Ferrari" }, new TeamEntity { teamName = "Scuda Ferrari" } };

            _teamServiceMock.Setup(p => p.Search(field, value, searchType)).Returns(expected);

            var result = _teamService.Search(field, value, searchType);

            Assert.That(result == expected);
        }
        [Test]
        public void SearchTeam_ReturnNull_WhenTeamDoesNotExist_Equals()
        {
            var field = "name";
            var value = "AMG Mercedes";
            var searchType = "e";

            _teamServiceMock.Setup(p => p.Search(field, value, searchType)).Returns((List<TeamEntity>)null);
            var result = _teamService.Search(field, value, searchType);
            Assert.That(result == null);
        }

        [Test]
        public void UpdateWhenTeamExist()
        {
            
            var team = new TeamEntity{teamName="test teamname",teamPrincipal="test principal", headquarters="test headquarters",constructorsChampionshipWins=1,year=2025};
            _teamServiceMock.Setup(p => p.Update(team)).Returns(true);
            var result = _teamService.Update(team);
            Assert.That(result == true);
        }
        [Test]
        public void UpdateWhenTeamDoesNotExist()
        {

            _teamServiceMock.Setup(p => p.Update((TeamEntity)null)).Returns(false);
            var result = _teamService.Update((TeamEntity)null);
            Assert.That(result == false);
        }
        [Test]
        public void DeleteWhenTeamExist()
        {
            var id = "testid";
            var team = new TeamEntity {Id=id, teamName = "test teamname", teamPrincipal = "test principal", headquarters = "test headquarters", constructorsChampionshipWins = 1, year = 2025 };
            _teamServiceMock.Setup(p => p.Delete(id)).Returns(true);
            var result = _teamService.Delete(id);
            Assert.That(result == true);
        }
        [Test]
        public void DeleteWhenTeamDoesNotExist()
        {

            _teamServiceMock.Setup(p => p.Delete(null)).Returns(false);
            var result = _teamService.Delete(null);
            Assert.That(result == false);
        }
    }
}
