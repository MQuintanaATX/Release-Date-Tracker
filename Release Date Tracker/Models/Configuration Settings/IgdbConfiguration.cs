namespace Release_Date_Tracker.Models.Configuration_Settings
{
    /// <summary>
    /// Contains the informaiton needed to register the IGdb Client
    /// </summary>
    public class IgdbConfiguration
    {
        public string ConfigSection => "Igdb";
        public required string ClientId { get; set; } 
        public required string ClientSecret { get; set;}
    }
}
