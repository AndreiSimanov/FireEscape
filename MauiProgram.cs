using DevExpress.Maui;
using MetroLog.MicrosoftExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace FireEscape;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        using var stream = GetStreamFromFile("appsettings.json");
        var configuration = new ConfigurationBuilder().AddJsonStream(stream!).Build();
        builder.Configuration.AddConfiguration(configuration);

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All // Customizing JsonConvert for SQLiteNetExtensions TextBlob
        };

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseAppSettings(configuration)
            .UseDevExpress(true)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        DevExpress.Maui.CollectionView.Initializer.Init();
        DevExpress.Maui.Controls.Initializer.Init();
        DevExpress.Maui.Editors.Initializer.Init();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Logging.
            AddTraceLogger(options =>
            {
                options.MinLevel = LogLevel.Trace;
                options.MaxLevel = LogLevel.Critical;
            }).
            AddInMemoryLogger(options =>
            {
                options.MaxLines = 1024;
                options.MinLevel = LogLevel.Debug;
                options.MaxLevel = LogLevel.Critical;
             }).
             AddStreamingFileLogger(options =>
             {
                 options.RetainDays = 2;
                 options.FolderPath = ApplicationSettings.LogFolder;
             });

        builder.Services.ConfigureServices();
        return builder.Build();
    }

    static Stream? GetStreamFromFile(string filename)
    {
        var assembly = typeof(App).GetTypeInfo().Assembly;
        var assemblyName = assembly.GetName().Name;
        var stream = assembly.GetManifestResourceStream($"{assemblyName}.{filename}");
        return stream;
    }
}