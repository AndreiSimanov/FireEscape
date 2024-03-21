namespace FireEscape.AppSettings
{
    public class DropboxSettings
    {
        public required string TokenUri { get; set; }
        public required string AppKey { get; set; }
        public required string AppSecret { get; set; }
        public required string RefreshToken { get; set; }
        public required string FolderName { get; set; }
    }
}
