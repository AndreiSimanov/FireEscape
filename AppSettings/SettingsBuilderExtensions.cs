namespace FireEscape.AppSettings
{
    public static class SettingsBuilderExtensions
    {
        private const string APPLICATION_SETTINGS = "ApplicationSettings";
        private const string FILE_HOSTING_SETTINGS = "FileHostingSettings";
        private const string NEW_PROTOCOL_SETTINGS = "NewProtocolSettings";
        private const string STAIRS_SETTINGS = "StairsSettings";

        public static MauiAppBuilder UseAppSettings(this MauiAppBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<ApplicationSettings>(options => configuration.GetSection(APPLICATION_SETTINGS).Bind(options));
            builder.Services.Configure<FileHostingSettings>(options => configuration.GetSection(FILE_HOSTING_SETTINGS).Bind(options));
            builder.Services.Configure<NewProtocolSettings>(options => configuration.GetSection(NEW_PROTOCOL_SETTINGS).Bind(options));
            builder.Services.Configure<StairsSettings>(options => configuration.GetSection(STAIRS_SETTINGS).Bind(options));
            return builder;
        }
    }
}