namespace FireEscape.AppSettings
{
    public class ApplicationSettings
    {
        public required string UserAccountsFolderName { get; set; }
        public int NewUserAccountExpirationDays { get; set; }
        public required string PrimaryThemeColor { get; set; }
        
    }
}
