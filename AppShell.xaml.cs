namespace FireEscape
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RemoteLogPage), typeof(RemoteLogPage));
            Routing.RegisterRoute(nameof(BatchReportPage), typeof(BatchReportPage));
            Routing.RegisterRoute(nameof(StairsPage), typeof(StairsPage));
            Routing.RegisterRoute(nameof(ProtocolPage), typeof(ProtocolPage));
            Routing.RegisterRoute(nameof(UserAccountMainPage), typeof(UserAccountMainPage));
            Routing.RegisterRoute(nameof(UserAccountPage), typeof(UserAccountPage));
            Routing.RegisterRoute(nameof(ProtocolMainPage), typeof(ProtocolMainPage));
            Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));
            Routing.RegisterRoute(nameof(OrderMainPage), typeof(OrderMainPage));
        }
    }
}
