
using IGDB;
using IGDB.Models;
using NodaTime;
using NodaTime.Extensions;
using Release_Date_Tracker.Clients;
using Release_Date_Tracker.Models;
using Release_Date_Tracker.Models.Configuration_Settings;
using System.Text.RegularExpressions;

namespace Release_Date_Tracker.Accessors
{
    public class IgdbAccessor : IIgdbAccessor
    {
        private ITwitchClient _iGDBClient;
        private readonly IClock _clock;

        private readonly GameTitles _gameTitles = new();

        public IgdbAccessor(ITwitchClient iGDBClient, IClock clock)
        {
            _iGDBClient = iGDBClient;
            _clock = clock;
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
            var gameResponses = await _iGDBClient.QueryGamesAsync(queryString);
            var games = new List<Game>(gameResponses.ToList());

            // The API only returns a max of 500 results; an offset value is used for repeated queries.
            var offset = 0;
            while (gameResponses.Count() == 500)
            {
                offset += 500;
                gameResponses = await _iGDBClient.QueryGamesAsync($"{queryString} offset {offset};");
                games.AddRange(gameResponses.ToList());
            }

            // Translates the results to the local model we use
            var tasks = games.Select(x => ConvertToGameTitleAsync(x, platforms));
            var gameTitles = await Task.WhenAll(tasks);

            // The API may add things as we retrieve them; we'll make sure not to introduce duplicates into a list
            _gameTitles.Titles = gameTitles.GroupBy(x => x.Id).Select(x => x.First()).ToDictionary(x => x.Id);

            _gameTitles.LastRetrievedDate = _clock.GetCurrentInstant().ToDateTimeUtc();
            return _gameTitles;
        }

        private Task<GameTitle> ConvertToGameTitleAsync(Game game, Dictionary<long, string> platforms)
        {
            var platformNames = new List<string>();
            foreach (var id in game.Platforms.Ids)
            {
                platforms.TryGetValue(id, out var platformName);
                // If we are missing a localization, instead add a strng showing the ID for later troubleshooting
                platformNames.Add(platformName ?? $"Platform ID: {id}");
            }

            return Task.FromResult(new GameTitle()
            {
                Id = (long)game.Id,
                Title = game.Name,
                ReleaseDate = game.FirstReleaseDate,
                Platforms = platformNames,
                Description = game.Summary
            });
        }

        // Gets a list of Platforms from the API; this will help provide readable results to the end user
        private async Task<Dictionary<long, string>> GetPlatformInformationAsync(List<long?> platformIds)
        {
            var platforms = new Dictionary<long,string>();
            var platformResponses = await _iGDBClient.QueryPlatformFamiliesAsync("fields *; limit 500;");

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
            var platformFamilies = await _iGDBClient.QueryPlatformFamiliesAsync("fields *; limit 500;");
            var familyIds = platformFamilies.Select(x => x.Id).ToList();
            var queryString = $"fields *; limit 500; where platform_family =  ({string.Join(",", familyIds)});";
            var platforms = await _iGDBClient.QueryPlatformsAsync(queryString);
            var platformIds = platforms.Select(x => x.Id).ToList();
            // 6 is the platform ID for PC.
            platformIds.Add(6);
            return platformIds;
        }
    }
}
