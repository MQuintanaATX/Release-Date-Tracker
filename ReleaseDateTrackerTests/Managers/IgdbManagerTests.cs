using AutoFixture;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using NSubstitute;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Managers;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.ClientModels;

namespace ReleaseDateTrackerTests.Managers
{
    public class IgdbManagerTests
    {
        private readonly IClock _clockMock;
        private readonly IGamesAccessor _gamesClient = Substitute.For<IGamesAccessor>();
        private readonly IPlatformFamiliesAccessor _platformFamiliesClient = Substitute.For<IPlatformFamiliesAccessor>();
        private readonly IPlatformsAccessor _platformsClient = Substitute.For<IPlatformsAccessor>();

        private Fixture _fixture = new Fixture();

        private IgdbManager _sut;
        public IgdbManagerTests()
        {
            _clockMock = new FakeClock(Instant.FromUtc(2000, 1, 1, 0, 0));

            _sut = new IgdbManager(_clockMock, _gamesClient, _platformFamiliesClient, _platformsClient);
        }

        [Test]
        public async Task GetGameTitlesAsync_Returns_Expected()
        {
            /* Arrange */
            var games = _fixture.Build<Game>()
                .With(x => x.PlatformIds, new List<long> { 1 })
                .CreateMany(100)
                .ToArray();

            _gamesClient.FilterAsync(Arg.Any<string>())
                .Returns(games);

            _platformsClient.FilterAsync(Arg.Any<string>())
                .Returns(new Platform[]
                {
                    new()
                    {
                        Id = 1,
                        Name = "Platform",
                        PlatformFamily = 100
                    }
                });

            _platformFamiliesClient.FilterAsync(Arg.Any<string>())
                .Returns(new PlatformFamily[]
                {
                    new PlatformFamily()
                    {
                        Id = 100,
                        Name = "Platform Family",
                    }
                });

            var titles = new Dictionary<long, GameTitle>();

            foreach (var game in games)
            {

                var gameTitle = new GameTitle
                {
                    Id = game.Id,
                    Title = game.Name,
                    ReleaseDate = game.FirstReleaseDate,
                    Platforms = new List<string> { "Platform" },
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
