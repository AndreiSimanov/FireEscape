﻿namespace FireEscape.AppSettings
{
    public class ApplicationSettings
    {
        public required string UserAccountsFolderName { get; set; }
        public int NewUserAccountExpirationDays { get; set; }
        public int NewUserAccountExpirationCount { get; set; }
        public required string PrimaryThemeColor { get; set; }

        string contentFolder = string.Empty;
        public required string ContentFolder
        {
            get => string.IsNullOrWhiteSpace(contentFolder) ? AppUtils.DefaultContentFolder : contentFolder;
            set => contentFolder = value;
        }
    }
}
