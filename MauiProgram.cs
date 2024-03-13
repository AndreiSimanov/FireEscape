﻿using Microsoft.Extensions.Logging;

namespace FireEscape
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif



            builder.Services.AddSingleton<ProtocolService>();
            builder.Services.AddSingleton<ProtocolsViewModel>();
            builder.Services.AddSingleton<MainPage>();

            //builder.Services.AddTransient<ProtocolDetailsViewModel>();
            builder.Services.AddTransient<ProtocolDetailsPage>();

            return builder.Build();
        }
    }
}
