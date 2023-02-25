using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Managers
{
    public interface IGameTitleManager
    {
        public Task<List<GameTitle>> GetAllGames();
    }
}
