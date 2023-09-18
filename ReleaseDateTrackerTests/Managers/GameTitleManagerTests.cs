using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;

namespace ReleaseDateTrackerTests.Managers
{
    public class GameTitleManagerTests
    {
        private IIgdbAccessor _accessorMock  = Substitute.For<IIgdbAccessor>();

        private GameTitleManager _sut;

        private Fixture _fixture = new ();

        public GameTitleManagerTests()
        {
            _sut = new GameTitleManager(_accessorMock);
        }

        [Test]
        public async Task GetAllTitlesAsync_Returns_Expected()
        {
            /* Arrange */
            var gameTitles = _fixture.CreateMany<GameTitle>()
                .ToList();

            var accessorTitles = gameTitles.ToDictionary(x => x.Id, 
                x => new GameTitle { 
                    Id = x.Id,
                    Description = x.Description, 
                    Platforms = x.Platforms, 
                    ReleaseDate = x.ReleaseDate, 
                    Title = x.Title
                });

            _accessorMock.GetGameAllTitlesAsync()
                .Returns(new GameTitles
                {
                    Titles = accessorTitles
                });

            /* Act */
            var actualGameTitles = await  _sut.GetAllGames();

            /* Assert */
            actualGameTitles.Should().BeEquivalentTo(gameTitles);
        }
    }
}
