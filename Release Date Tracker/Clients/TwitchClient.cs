using IGDB;
using IGDB.Models;
using Release_Date_Tracker.Models.Configuration_Settings;
using RestEase;
using System.Net;

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
            while (attempts  < 5)
            {
                try
                {
                    return await _iGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query);
                }
                catch (ApiException ex)
                {
                    attempts++;
                    System.Threading.Thread.Sleep(1000);
                }
            }
            throw new Exception("TOo Many Requests");
        }

        public async Task<PlatformFamily[]> QueryPlatformFamiliesAsync(string query)
        {
            return await _iGDBClient.QueryAsync<PlatformFamily>(IGDBClient.Endpoints.PlatformFamilies, query);
        }

        public async Task<Platform[]> QueryPlatformsAsync(string query)
        {
            return await _iGDBClient.QueryAsync<Platform>(IGDBClient.Endpoints.Platforms, query);
        }
    }
}
