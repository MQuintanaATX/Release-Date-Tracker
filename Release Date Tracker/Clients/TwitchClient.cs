using IGDB;
using Release_Date_Tracker.Models.Configuration_Settings;
using RestEase;
using Release_Date_Tracker.Models.ClientModels;

namespace Release_Date_Tracker.Clients
{
    public class TwitchClient : ITwitchClient
    {
        private readonly IGDBClient _iGDBClient;

        public TwitchClient(IgdbConfiguration configuration)
        {
            _iGDBClient = new IGDBClient(configuration.ClientId, configuration.ClientSecret);
        }
        public async Task<Game[]> QueryGamesAsync(string query)
        {
            var attempts = 0;
            while (attempts < 5)
            {
                try
                {
                    var clientGames = await _iGDBClient.QueryAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games, query: query);
                    return clientGames.Select(x =>
                    {
                        return new Game
                        {
                            Id = x.Id ?? 0,
                            Title = x.Name,
                            Summary = x.Summary,
                            FirstReleaseDate = x.FirstReleaseDate ?? DateTimeOffset.MinValue,
                            PlatformIds = x.Platforms.Ids.ToList(),
                            Name = x.Name
                        };
                    }
                    ).ToArray();
                }
                catch (ApiException)
                {
                    attempts++;
                    Thread.Sleep(1000);
                }
            }
            throw new Exception("TOo Many Requests");
        }

        public async Task<PlatformFamily[]> QueryPlatformFamiliesAsync(string query)
        {
            var clientPlatformFamilies = await _iGDBClient.QueryAsync<IGDB.Models.PlatformFamily>(IGDBClient.Endpoints.PlatformFamilies, query);

            return clientPlatformFamilies.Select(x =>
            {
                return new PlatformFamily { Name = x.Name, Id = (int)(x.Id ?? 0) };
            }).ToArray();
        }

        public async Task<Platform[]> QueryPlatformsAsync(string query)
        {
            var clientPlatforms = await _iGDBClient.QueryAsync<IGDB.Models.Platform>(IGDBClient.Endpoints.Platforms, query);
            return clientPlatforms.Select(x =>
            {
                return new Platform
                {
                    Name = x.Name,
                    Id = (int)(x.Id ?? 0),
                    PlatformFamily = x.PlatformFamily?.Id ?? 0
                };
            }).ToArray();
        }
    }
}
