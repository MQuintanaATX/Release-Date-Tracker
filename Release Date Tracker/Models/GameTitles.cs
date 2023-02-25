namespace Release_Date_Tracker.Models
{
    public class GameTitles
    {
        public Dictionary<int, GameTitle> Titles { get; set; } = new Dictionary<int, GameTitle> { { 123, new GameTitle() } };
    }
}
