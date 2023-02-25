using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Accessors
{
    public interface IIgdbAccessor
    {
        public Task<GameTitles> GetGameTitlesAsync();
    }
}
