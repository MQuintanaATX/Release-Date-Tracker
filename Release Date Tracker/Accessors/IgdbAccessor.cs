
using IGDB;
using IGDB.Models;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.Configuration_Settings;

namespace Release_Date_Tracker.Accessors
{
    public class IgdbAccessor : IIgdbAccessor
    {
        private readonly IGDBClient _iGDBClient;

        private readonly GameTitles _gameTitles = new();

        public IgdbAccessor(IgdbConfiguration configuration)
        {
            _iGDBClient = new IGDBClient(configuration.ClientId, configuration.ClientSecret);
        }

        public async Task<GameTitles> GetGameAllTitlesAsync()
        {
            // Check to see if the data is stored in memory, and is recent; if so, return the existing values
            if (_gameTitles.LastRetrievedDate > DateTime.Now.AddDays(-7))
            {
                return _gameTitles;
            }

            // Get Platform information to display user friendly names later, and to refine results from API
            var platformIds = await GetPlatformIds();

            var platforms = await GetPlatformInformationAsync(platformIds);

            // The project assumes a user will want to look up to one year in the past
            var date = DateTimeOffset.UtcNow.AddYears(-1).ToUnixTimeSeconds();

            // Get a list of games for PC, Xbox, Sony, and Nintendo clients
            // Gets a living list of platform IDs; this will catch future console releases from the big three and for PCs
            var queryString = $"fields id, name, first_release_date, summary, platforms; limit 500; where first_release_date > {date} & platforms = ({string.Join(",", platformIds.Where(x => x != null))});";
            var gameResponses = await _iGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: queryString);
            var games = new List<Game>(gameResponses.ToList());

            // The API only returns a max of 500 results; an offset value is used for repeated queries.
            var offset = 0;
            while (gameResponses.Count() == 500)
            {
                offset += 500;
                gameResponses = await _iGDBClient.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: $"{queryString} offset {offset};");
                games.AddRange(gameResponses.ToList());
            }
            
            foreach(var game in games)
            {
                // Prevent adding a game title with a null id, or an existing entry. 
                // Existing entries can occur if the API returns a slightly different result set due to addition or removal of entries
                if (game.Id is null || _gameTitles.Titles.ContainsKey((long)game.Id)) continue;

                // Get Platform Information
                var platformNames = new List<string>();
                foreach (var id in game.Platforms.Ids)
                {
                    platforms.TryGetValue(id, out var platformName);
                    // If we are missing a localization, instead add a strng showing the ID for later troubleshooting
                    platformNames.Add(platformName ?? $"Platform ID: {id}");
                }

                var gameTitle = new GameTitle()
                {
                    Id = (long)game.Id,
                    Title = game.Name,
                    ReleaseDate = game.FirstReleaseDate,
                    Platforms = platformNames,
                    Description = game.Summary
                };
            }
            
            _gameTitles.LastRetrievedDate = DateTime.Now;
            return _gameTitles;
        }

        // Gets a list of Platforms from the API; this will help provide readable results to the end user.
        private async Task<Dictionary<long, string>> GetPlatformInformationAsync(List<long?> platformIds)
        {
            var platforms = new Dictionary<long,string>();
            var platformResponses = await _iGDBClient.QueryAsync<Platform>(IGDBClient.Endpoints.Platforms, query: "fields *; limit 500;");

            foreach(var platform in platformResponses.Where(x => platformIds.Contains(x.Id)))
            {
                if (platform.Id is null) continue;
                platforms.Add((long)platform.Id, platform.Name);
            }

            return platforms;
        }

        // Get a list of platorm IDs that belong to the big three and PC platforms. 
        private async Task<List<long?>> GetPlatformIds()
        {
            var platformFamilies = await _iGDBClient.QueryAsync<PlatformFamily>(IGDBClient.Endpoints.PlatformFamilies);
            var familyIds = platformFamilies.Select(x => x.Id).ToList();
            var queryString = $"fields *; limit 500; where platform_family =  ({string.Join(",", familyIds)});";
            var platforms = await _iGDBClient.QueryAsync<Platform>(IGDBClient.Endpoints.Platforms, query: queryString);
            var platformIds = platforms.Select(x => x.Id).ToList();
            // 6 is the platform ID for PC.
            platformIds.Add(6);
            return platformIds;
        }
    }
}
