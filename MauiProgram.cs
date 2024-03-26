﻿using DevExpress.Maui;
using DevExpress.Maui.Core;
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
            builder.Services.AddSingleton<IProtocolRepository, ProtocolRepository>();
            builder.Services.AddSingleton<IReportRepository, PdfWriterRepository>();
            builder.Services.AddSingleton<IFileHostingRepository, DropboxRepository>();

            builder.Services.AddSingleton<UserAccountService>();
            builder.Services.AddSingleton<ProtocolService>();

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<UserAccountsViewModel>();
            builder.Services.AddSingleton<UserAccountsPage>();

            builder.Services.AddTransient<UserAccountViewModel>();
            builder.Services.AddTransient<UserAccountPage>();

            builder.Services.AddTransient<ProtocolViewModel>();
            builder.Services.AddTransient<ProtocolPage>();

            builder.Services.AddTransient<FireEscapeViewModel>();
            builder.Services.AddTransient<FireEscapePage>();

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