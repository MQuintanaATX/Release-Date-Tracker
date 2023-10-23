using FluentAssertions;
using NSubstitute;
using Release_Date_Tracker.Controllers;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace ReleaseDateTrackerTests.Controllers
{
    public class GameTitleControllerTests
    {
        private readonly IIgdbManager _igdbManager = Substitute.For<IIgdbManager>();

        private readonly GameTitleController _sut;

        public GameTitleControllerTests() 
        { 
            _sut = new GameTitleController(_igdbManager);
        }

        [Test]
        public async Task GetAllGames_CallsManager()
        {
            /* Arrange */
            var titles = new GameTitles
            {
                Titles = new Dictionary<long, GameTitle>
                {
                    {1, new GameTitle{ Title = "My Name", Id = 1 }}
                },
                LastRetrievedDate = DateTime.UtcNow,
            };

            var expectedGameTitles = titles.Titles.Values.ToList();
            _igdbManager.GetGameAllTitlesAsync()
                .Returns(titles);

            /* Act */
            var actualGameTitles = await _sut.GetAllTitlesAsync();

            /* Assert */
            actualGameTitles.Should().BeEquivalentTo(expectedGameTitles);
            await _igdbManager.Received().GetGameAllTitlesAsync();
        }
    }
}
