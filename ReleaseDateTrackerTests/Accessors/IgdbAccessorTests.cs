using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using NSubstitute;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Clients;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.ClientModels;

namespace ReleaseDateTrackerTests.Accessors
{
    public class IgdbAccessorTests
    {
        private readonly IClock _clockMock;
        private readonly ITwitchClient _clientMock = Substitute.For<ITwitchClient>();

        private Fixture _fixture = new Fixture();

        private IgdbAccessor _sut;
        public IgdbAccessorTests()
        {
            _clockMock = new FakeClock(Instant.FromUtc(2000, 1, 1, 0, 0));

            _sut = new IgdbAccessor(_clientMock, _clockMock);
        }

        [Test]
        public async Task GetGameTitlesAsync_Returns_Expected()
        {
            /* Arrange */
            var games = _fixture.Build<Game>()
                .With(x => x.PlatformIds, new List<long> { 1 })
                .CreateMany(100)
                .ToArray();

            _clientMock.QueryGamesAsync(Arg.Any<string>())
                .Returns(games);

            _clientMock.QueryPlatformsAsync(Arg.Any<string>())
                .Returns(new Platform[]
                {
                    new()
                    {
                        Id = 1,
                        Name = "Platform",
                        PlatformFamily = 100
                    }
                });

            _clientMock.QueryPlatformFamiliesAsync(Arg.Any<string>())
                .Returns(new PlatformFamily[]
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
