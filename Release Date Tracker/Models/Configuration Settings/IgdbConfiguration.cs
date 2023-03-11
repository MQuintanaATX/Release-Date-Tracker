namespace Release_Date_Tracker.Models.Configuration_Settings
{
    /// <summary>
    /// Contains the informaiton needed to register the IGdb Client
    /// </summary>
    public class IgdbConfiguration
    {
        public string ConfigSection => "Igdb";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set;} = string.Empty;
    }
}
