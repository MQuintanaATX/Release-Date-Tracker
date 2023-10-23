using Release_Date_Tracker.Models.ClientModels;

namespace Release_Date_Tracker.Accessors;

public interface IPlatformFamiliesAccessor
{
    Task<PlatformFamily[]> FilterAsync(string query);
}