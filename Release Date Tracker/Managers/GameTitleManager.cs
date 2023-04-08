using Release_Date_Tracker.Accessors;
using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Managers
{
    public class GameTitleManager : IGameTitleManager
    {
        private readonly IIgdbAccessor _igdbAccessor;

        public GameTitleManager(IIgdbAccessor igdbAccessor)
        {
            _igdbAccessor = igdbAccessor;
        }

        public async Task<List<GameTitle>> GetAllGames()
        {
            var gameTitlesDictionary = await _igdbAccessor.GetGameAllTitlesAsync();
            return gameTitlesDictionary.Titles.Values.ToList();
        }
    }
}
