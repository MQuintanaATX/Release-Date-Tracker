using AutoFixture;
using IGDB;
using IGDB.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Testing;
using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Clients;
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
            var games = _fixture.Build<Game>()
                .With(x => x.Platforms.Ids, new long [1])
                .CreateMany(100)
                .ToArray();

            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                                              ItExpr.IsAny<HttpRequestMessage>(),
                                              ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(games))
                });

            _clientMock.Setup(x => x.QueryGamesAsync(It.IsAny<string>()))
                .ReturnsAsync(games);

            _clientMock.Setup(x => x.QueryPlatformsAsync(It.IsAny<string>()))
                .ReturnsAsync(new PlatformFamily[]
                {
                    new PlatformFamily
                    {
                        Id = 1
                    }
                });

            /* Act */
            var actualGameTitles = await _sut.GetGameAllTitlesAsync();
            /* Assert*/
        }
    }
}
