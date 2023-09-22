using Release_Date_Tracker.Models;

namespace Release_Date_Tracker.Managers;

public interface IIgdbManager
{
    public Task<GameTitles> GetGameAllTitlesAsync();
}