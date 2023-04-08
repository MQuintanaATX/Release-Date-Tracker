using IGDB;
using IGDB.Models;

namespace Release_Date_Tracker.Models
{
    public class GameTitle
    {
        public required string Title { get; init; } 
        public long Id { get; init; }
        public DateTimeOffset? ReleaseDate { get; init; }
        public List<string> Platforms { get; init; } = new List<string>();
        public string Description { get; init; } = string.Empty;
    }
}
