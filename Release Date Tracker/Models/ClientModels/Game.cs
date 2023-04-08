using IGDB.Models;

namespace Release_Date_Tracker.Models.ClientModels
{
    public class Game
    {
        public required long Id { get; set; }
        public required string Title { get; set; }
        public required string Summary { get; set; }
        public required List<long> PlatformIds { get; set; }
        public required DateTimeOffset FirstReleaseDate { get; set; }
        public required string Name { get; set; }
    }
}
