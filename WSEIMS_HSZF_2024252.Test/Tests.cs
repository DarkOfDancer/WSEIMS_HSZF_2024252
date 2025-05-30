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
        public void Search_ReturnList_WhenTeamsExist_Contains()
        {
            // Arrange
            var field = "name";
            var value = "ferrari";
            var searchType = "c";

            var teamsInDb = new List<TeamEntity>
    {
        new TeamEntity { teamName = "Scuderia Ferrari" },
        new TeamEntity { teamName = "Red Bull Racing" },
        new TeamEntity { teamName = "BWT Mercedes" }
    };

            _teamServiceMock.Setup(dp => dp.GetAll()).Returns(teamsInDb);

            // Act
            var result = _teamService.Search(field, value, searchType);

            // Assert
            Assert.That(result.Any(t => t.teamName?.ToLower().Contains(value) == true), Is.True);
        }

        [Test]
        public void Search_ReturnEmpty_WhenTeamsDoesNotExist_Contains()
        {
            var field = "name";
            var value = "Ferrari";
            var searchType = "c"; 

            _teamServiceMock.Setup(dp => dp.GetAll()).Returns((List<TeamEntity>)null);

            var result = _teamService.Search(field, value, searchType);
      
            Assert.That(result.Count==0);   
        }
        [Test]
        public void Search_ReturnList_WhenTeamsExist_Equals()
        {
            var field = "name";
            var value = "Scuderia Ferrari";
            var searchType = "e";

            var teamsInDb = new List<TeamEntity>
    {
        new TeamEntity { teamName = "Scuderia Ferrari" },
        new TeamEntity { teamName = "Red Bull Racing" },
        new TeamEntity { teamName = "BWT Mercedes" }
    };

            _teamServiceMock.Setup(dp => dp.GetAll()).Returns(teamsInDb);

            var result = _teamService.Search(field, value, searchType);

            Assert.That(result.Any(t => t.teamName.Equals(value))==true);
        }
        [Test]
        public void Search_ReturnEmpty_WhenTeamsDoesNotExist_Equals()
        {
            var field = "name";
            var value = "Scuderia Ferrari";
            var searchType = "e";


            _teamServiceMock.Setup(dp => dp.GetAll()).Returns((List<TeamEntity>)null);

            var result = _teamService.Search(field, value, searchType);

            Assert.That(result.Count==0);
        }

        [Test]
        public void UpdateWhenTeamExist()
        {
            var teamId = "team123";
            var team = new TeamEntity
            {
                Id = teamId,
                teamName = "test teamname",
                teamPrincipal = "test principal",
                headquarters = "test headquarters",
                constructorsChampionshipWins = 1,
                year = 2025
            };

            _teamServiceMock.Setup(dp => dp.GetById(teamId)).Returns(team);

            _teamServiceMock.Setup(dp => dp.Update(It.Is<TeamEntity>(t => t.Id == teamId))).Returns(true);

            var result = _teamService.Update(team);

            Assert.That(result==true);
        }
        [Test]
        public void UpdateWhenTeamDoesNotExist_ReturnsFalse()
        {
            var teamId = "nonexistentId";
            var team = new TeamEntity
            {
                Id = teamId,
                teamName = "Nonexistent Team",
                teamPrincipal = "No Principal",
                headquarters = "Nowhere",
                constructorsChampionshipWins = 0,
                year = 2025
            };

            _teamServiceMock.Setup(dp => dp.GetById(teamId)).Returns((TeamEntity)null);

            var result = _teamService.Update(team);

            Assert.That(result==false);
        }
        [Test]
        public void DeleteWhenTeamExists()
        {
            // Arrange
            var teamId = "test-team-id";

            var dummyTeam = new TeamEntity
            {
                Id = teamId,
                teamName = "Test Team",
                budget = new BudgetEntity
                {
                    expenses = new List<ExpensEntity>
            {
                new ExpensEntity
                {
                    subcategory = new List<SubcategoryEntity>
                    {
                        new SubcategoryEntity { Id = "sub1", name = "Sub1" }
                    }
                }
            }
                }
            };

            _teamServiceMock.Setup(dp => dp.GetById(teamId)).Returns(dummyTeam);
            _teamServiceMock.Setup(dp => dp.Delete(teamId)).Returns(true);

            // Act
            var result = _teamService.Delete(teamId);

            // Assert
            Assert.That(result==true);
        }
        [Test]
        public void DeleteWhenTeamDoesNotExist()
        {
            var teamId = "non-existent-id";

            _teamServiceMock.Setup(dp => dp.GetById(teamId)).Returns((TeamEntity)null);

            var result = _teamService.Delete(teamId);

            Assert.That(result==false);
        }
    }
}
