using Release_Date_Tracker.Models.ClientModels;

namespace Release_Date_Tracker.Clients
{
    public interface ITwitchClient
    {
        Task<Game[]> QueryGamesAsync(string query);
        Task<PlatformFamily[]> QueryPlatformFamiliesAsync(string query);
        Task<Platform[]> QueryPlatformsAsync(string query);
    }
}
