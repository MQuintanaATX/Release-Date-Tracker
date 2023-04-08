namespace Release_Date_Tracker.Models.ClientModels
{
    public class Platform
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required long PlatformFamily { get; set; }
    }
}
