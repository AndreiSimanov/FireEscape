using FireEscape.DBContext;
using FireEscape.Factories;
using FireEscape.Factories.Interfaces;
using FireEscape.Reports.Interfaces;
using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportMakers;
using FireEscape.Validators;
using FluentValidation;

namespace FireEscape;

public static class ModuleInitializer
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<SqliteContext>();

        services.AddTransient<IValidator<Stairs>, StairsValidator>();

        services.AddSingleton<IOrderFactory, OrderFactory>();
        services.AddSingleton<IProtocolFactory, ProtocolFactory>();
        services.AddSingleton<IStairsFactory, StairsFactory>();

        services.AddSingleton<ISearchDataRepository, SearchDataRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IProtocolRepository, ProtocolRepository>();
        services.AddSingleton<IStairsRepository, StairsRepository>();
        services.AddSingleton<IFileHostingRepository, DropboxRepository>();
        services.AddSingleton<IReportRepository, PdfWriterRepository>();
        services.AddSingleton<IProtocolPdfReportMaker, ProtocolPdfReportMaker>();
        services.AddSingleton<IProtocolReportDataProvider, ProtocolReportDataProvider>();

        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<IProtocolService, ProtocolService>();
        services.AddSingleton<IStairsService, StairsService>();
        services.AddSingleton<IUserAccountService, UserAccountService>();
        services.AddSingleton<IRemoteLogService, RemoteLogService>();

        services.AddTransient<OrderMainViewModel>();
        services.AddTransient<OrderViewModel>();
        services.AddTransient<ProtocolMainViewModel>();
        services.AddTransient<ProtocolViewModel>();
        services.AddTransient<StairsViewModel>();
        services.AddTransient<UserAccountMainViewModel>();
        services.AddTransient<UserAccountViewModel>();
        services.AddTransient<BatchReportViewModel>();
        services.AddTransient<RemoteLogViewModel>();

        services.AddSingleton<OrderMainPage>();
        services.AddSingleton<OrderPage>();
        services.AddSingleton<ProtocolMainPage>();
        services.AddSingleton<ProtocolPage>();
        services.AddSingleton<StairsPage>();
        services.AddSingleton<UserAccountMainPage>();
        services.AddSingleton<UserAccountPage>();
        services.AddSingleton<BatchReportPage>();
        services.AddSingleton<RemoteLogPage>();

        return services;
    }
}
