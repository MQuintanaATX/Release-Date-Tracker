namespace Release_Date_Tracker.Models
{
    public class GameTitles
    {
        public Dictionary<long, GameTitle> Titles { get; set; } = new Dictionary<long, GameTitle> {};
        public DateTime LastRetrievedDate { get; set; }

    }
}
