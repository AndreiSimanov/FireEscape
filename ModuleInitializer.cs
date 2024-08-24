using FireEscape.DBContext;
using FireEscape.Factories;
using FireEscape.Factories.Interfaces;
using FireEscape.Reports.ReportMakers;
using FireEscape.Validators;
using FluentValidation;

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
        services.AddSingleton<IProtocolPdfReportMaker, ProtocolPdfReportMaker>();

        services.AddSingleton<ReportService>();
        services.AddSingleton<OrderService>();
        services.AddSingleton<ProtocolService>();
        services.AddSingleton<StairsService>();
        services.AddSingleton<UserAccountService>();
        services.AddSingleton<RemoteLogService>();

        services.AddSingleton<OrderMainViewModel>();
        services.AddSingleton<OrderMainPage>();

        services.AddTransient<OrderViewModel>();
        services.AddSingleton<OrderPage>();

        services.AddTransient<ProtocolMainViewModel>();
        services.AddSingleton<ProtocolMainPage>();

        services.AddTransient<ProtocolViewModel>();
        services.AddSingleton<ProtocolPage>();

        services.AddTransient<StairsViewModel>();
        services.AddSingleton<StairsPage>();

        services.AddSingleton<UserAccountMainViewModel>();
        services.AddSingleton<UserAccountMainPage>();

        services.AddTransient<UserAccountViewModel>();
        services.AddSingleton<UserAccountPage>();

        services.AddTransient<BatchReportViewModel>();
        services.AddSingleton<BatchReportPage>();

        services.AddTransient<RemoteLogViewModel>();
        services.AddSingleton<RemoteLogPage>();

        services.AddTransient<IValidator<Stairs>, StairsValidator>();

        return services;
    }
}
