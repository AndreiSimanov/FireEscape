namespace FireEscape.AppSettings;

public static class SettingsBuilderExtensions
{
    const string APPLICATION_SETTINGS = "ApplicationSettings";
    const string REMOTE_LOG_SETTINGS = "RemoteLogSettings";
    const string REPORT_SETTINGS = "ReportSettings";
    const string FILE_HOSTING_SETTINGS = "FileHostingSettings";
    const string ORDER_SETTINGS = "OrderSettings";
    const string PROTOCOL_SETTINGS = "ProtocolSettings";
    const string STAIRS_SETTINGS = "StairsSettings";

    public static MauiAppBuilder UseAppSettings(this MauiAppBuilder builder, IConfiguration configuration)
    {
        builder.Services.Configure<ApplicationSettings>(options => configuration.GetSection(APPLICATION_SETTINGS).Bind(options));
        builder.Services.Configure<RemoteLogSettings>(options => configuration.GetSection(REMOTE_LOG_SETTINGS).Bind(options));
        builder.Services.Configure<ReportSettings>(options => configuration.GetSection(REPORT_SETTINGS).Bind(options));
        builder.Services.Configure<FileHostingSettings>(options => configuration.GetSection(FILE_HOSTING_SETTINGS).Bind(options));
        builder.Services.Configure<OrderSettings>(options => configuration.GetSection(ORDER_SETTINGS).Bind(options));
        builder.Services.Configure<ProtocolSettings>(options => configuration.GetSection(PROTOCOL_SETTINGS).Bind(options));
        builder.Services.Configure<StairsSettings>(options => configuration.GetSection(STAIRS_SETTINGS).Bind(options));
        return builder;
    }
}