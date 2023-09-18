using FluentAssertions;
using NSubstitute;
using Release_Date_Tracker.Controllers;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace ReleaseDateTrackerTests.Controllers
{
    public class GameTitleControllerTests
    {
        private readonly IGameTitleManager _gameTitleManagerMock = Substitute.For<IGameTitleManager>();

        private readonly GameTitleController _sut;

        public GameTitleControllerTests() 
        { 
            _sut = new GameTitleController(_gameTitleManagerMock);
        }

        [Test]
        public async Task GetAllGames_CallsManager()
        {
            /* Arrange */
            var expectedGameTitles = new List<GameTitle>
                {
                    new()
                    {
                       Title = "My name",
                       Id = 1,
                    }
                };

            _gameTitleManagerMock.GetAllGames()
                .Returns(expectedGameTitles);

            /* Act */
            var actualGameTitles = await _sut.GetAllTitlesAsync();

            /* Assert */
            actualGameTitles.Should().BeEquivalentTo(expectedGameTitles);
            await _gameTitleManagerMock.Received().GetAllGames();
        }
    }
}
