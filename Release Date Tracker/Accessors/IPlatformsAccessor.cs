using Release_Date_Tracker.Models.ClientModels;

namespace Release_Date_Tracker.Accessors;

public interface IPlatformsAccessor
{
    Task<Platform[]> FilterAsync(string query);
}