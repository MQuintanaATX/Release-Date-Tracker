using Release_Date_Tracker.Models.ClientModels;

namespace Release_Date_Tracker.Accessors;

public interface IGamesAccessor
{
    Task<Game[]> FilterAsync(string query);
}