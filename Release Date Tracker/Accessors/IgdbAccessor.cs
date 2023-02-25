using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Accessors
{
    public class IgdbAccessor : IIgdbAccessor
    {
        public Task<GameTitles> GetGameTitlesAsync()
        {
            //TODO: Implement Igdb API
            return Task.FromResult(new GameTitles());
        }
    }
}
