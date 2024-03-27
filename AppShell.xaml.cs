namespace FireEscape
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FireEscapePage), typeof(FireEscapePage));
            Routing.RegisterRoute(nameof(ProtocolPage), typeof(ProtocolPage));
            Routing.RegisterRoute(nameof(UserAccountsPage), typeof(UserAccountsPage));
            Routing.RegisterRoute(nameof(UserAccountPage), typeof(UserAccountPage));
            Routing.RegisterRoute(nameof(ProtocolsMainPage), typeof(ProtocolsMainPage));
        }
    }
}
