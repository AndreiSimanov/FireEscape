using FireEscape.DBContext;
using FireEscape.Factories;
using FireEscape.Factories.Interfaces;

namespace FireEscape;

public static class ModuleInitializer
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<SqliteContext>();

        services.AddSingleton<IOrderFactory, OrderFactory>();
        services.AddSingleton<IProtocolFactory, ProtocolFactory>();
        services.AddSingleton<IStairsFactory, StairsFactory>();

        services.AddSingleton<ISearchDataRepository, SearchDataRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IProtocolRepository, ProtocolRepository>();
        services.AddSingleton<IStairsRepository, StairsRepository>();
        services.AddSingleton<IReportRepository, PdfWriterRepository>();
        services.AddSingleton<IFileHostingRepository, DropboxRepository>();

        services.AddSingleton<OrderService>();
        services.AddSingleton<ProtocolService>();
        services.AddSingleton<UserAccountService>();

        services.AddSingleton<OrderMainViewModel>();
        services.AddSingleton<OrderMainPage>();

        services.AddTransient<OrderViewModel>();
        services.AddTransient<OrderPage>();

        services.AddTransient<ProtocolMainViewModel>();
        services.AddTransient<ProtocolMainPage>();

        services.AddTransient<ProtocolViewModel>();
        services.AddTransient<ProtocolPage>();

        services.AddTransient<StairsViewModel>();
        services.AddTransient<StairsPage>();

        services.AddSingleton<UserAccountMainViewModel>();
        services.AddSingleton<UserAccountMainPage>();

        services.AddTransient<UserAccountViewModel>();
        services.AddTransient<UserAccountPage>();

        return services;
    }
}
