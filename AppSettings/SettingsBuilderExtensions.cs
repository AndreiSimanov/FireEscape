namespace FireEscape.AppSettings
{
    public static class SettingsBuilderExtensions
    {
        const string APPLICATION_SETTINGS = "ApplicationSettings";
        const string FILE_HOSTING_SETTINGS = "FileHostingSettings";
        const string NEW_PROTOCOL_SETTINGS = "NewProtocolSettings";
        const string STAIRS_SETTINGS = "StairsSettings";

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