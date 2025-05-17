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
        private Mock<ITeamDataProvider> _teamDataProviderMock;

        private TeamService _teamService;

        [SetUp]
        public void Setup()
        {
            _teamDataProviderMock = new Mock<ITeamDataProvider>();
        }
    }
}
