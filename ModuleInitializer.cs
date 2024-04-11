using FireEscape.Factories;

namespace FireEscape
{
    public static class ModuleInitializer
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<ProtocolFactory>();
            services.AddSingleton<StairsFactory>();

            services.AddSingleton<IProtocolRepository, LocalFileRepository>();
            services.AddSingleton<IReportRepository, PdfWriterRepository>();
            services.AddSingleton<IFileHostingRepository, DropboxRepository>();

            services.AddSingleton<UserAccountService>();
            services.AddSingleton<ProtocolService>();

            services.AddSingleton<ProtocolMainViewModel>();
            services.AddSingleton<ProtocolMainPage>();

            services.AddSingleton<UserAccountMainViewModel>();
            services.AddSingleton<UserAccountMainPage>();

            services.AddTransient<UserAccountViewModel>();
            services.AddTransient<UserAccountPage>();

            services.AddTransient<ProtocolViewModel>();
            services.AddTransient<ProtocolPage>();

            services.AddTransient<StairsViewModel>();
            services.AddTransient<StairsPage>();

            return services;
        }
    }
}
