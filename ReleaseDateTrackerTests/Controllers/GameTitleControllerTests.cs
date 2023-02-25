using FluentAssertions;
using Moq;
using Release_Date_Tracker.Controllers;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace ReleaseDateTrackerTests.Controllers
{
    public class GameTitleControllerTests
    {
        private readonly Mock<IGameTitleManager> _gameTitleManagerMock = new ();

        private readonly GameTitleController _sut;

        public GameTitleControllerTests() 
        { 
            _sut = new GameTitleController(_gameTitleManagerMock.Object);
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

            _gameTitleManagerMock.Setup(x => x.GetAllGames())
                .ReturnsAsync(expectedGameTitles);

            /* Act */
            var actualGameTitles = await _sut.GetAllTitlesAsync();

            /* Assert */
            actualGameTitles.Should().BeEquivalentTo(expectedGameTitles);
            _gameTitleManagerMock.Verify(x => x.GetAllGames(), Times.Once());
        }
    }
}
