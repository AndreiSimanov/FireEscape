using DevExpress.Maui;
using FireEscape.Factories;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FireEscape
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            using var stream = GetStreamFromFile("appsettings.json");
            var configuration = new ConfigurationBuilder().AddJsonStream(stream!).Build();
            builder.Configuration.AddConfiguration(configuration);

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
}