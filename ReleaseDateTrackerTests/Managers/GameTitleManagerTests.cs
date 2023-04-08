using AutoFixture;
using FluentAssertions;
using Moq;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;
using System.Security.Cryptography.X509Certificates;

namespace ReleaseDateTrackerTests.Managers
{
    public class GameTitleManagerTests
    {
        private Mock<IIgdbAccessor> _accessorMock  = new();

        private GameTitleManager _sut;

        private Fixture _fixture = new ();

        public GameTitleManagerTests()
        {
            _sut = new GameTitleManager(_accessorMock.Object);
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

            _accessorMock.Setup(x => x.GetGameAllTitlesAsync())
                .ReturnsAsync(new GameTitles
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
