using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Testing;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Clients;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.ClientModels;
using Release_Date_Tracker.Models.Configuration_Settings;
using System.Net;
using Xunit;

namespace ReleaseDateTrackerTests.Accessors
{
    public class IgdbAccessorTests
    {
        private readonly IClock _clockMock;
        private readonly Mock<ITwitchClient> _clientMock = new();

        private Fixture _fixture = new Fixture();

        private IgdbAccessor _sut;
        public IgdbAccessorTests()
        {
            _clockMock = new FakeClock(Instant.FromUtc(2000, 1, 1, 0, 0));

            _sut = new IgdbAccessor(_clientMock.Object, _clockMock);
        }

        [Test]
        public async Task GetGameTitlesAsync_Returns_Expected()
        {
            /* Arrange */
            var platforms = new Mock<Platform>();

            var games = _fixture.Build<Game>()
                .With(x => x.PlatformIds, new List<long> { 1 })
                .CreateMany(100)
                .ToArray();

            _clientMock.Setup(x => x.QueryGamesAsync(It.IsAny<string>()))
                .ReturnsAsync(games);

            _clientMock.Setup(x => x.QueryPlatformsAsync(It.IsAny<string>()))
                .ReturnsAsync(new Platform[]
                {
                    new()
                    {
                        Id = 1,
                        Name = "Platform",
                        PlatformFamily = 100
                    }
                });

            _clientMock.Setup(x => x.QueryPlatformFamiliesAsync(It.IsAny<string>()))
                .ReturnsAsync(new PlatformFamily[]
                {
                    new PlatformFamily()
                    {
                        Id = 100,
                        Name = "Platform Family",
                    }
                });

            var titles = new Dictionary<long, GameTitle>();

            foreach(var game in games)
            {

                var gameTitle = new GameTitle
                {
                    Id = (long)game.Id,
                    Title = game.Name,
                    ReleaseDate = game.FirstReleaseDate,
                    Platforms = new List<string> { "Platform"},
                    Description = game.Summary
                };
                titles.Add(game.Id, gameTitle);
            }

            var expectedGameTitles = new GameTitles
            {
                LastRetrievedDate = _clockMock.GetCurrentInstant().ToDateTimeUtc(),
                Titles = titles
            };

            /* Act */
            var actualGameTitles = await _sut.GetGameAllTitlesAsync();

            /* Assert*/
            actualGameTitles.Should().BeEquivalentTo(expectedGameTitles);
        }
    }
}
