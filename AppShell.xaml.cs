namespace FireEscape
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FireEscapePage), typeof(FireEscapePage));
            Routing.RegisterRoute(nameof(ProtocolPage), typeof(ProtocolPage));
            Routing.RegisterRoute(nameof(UserAccountMainPage), typeof(UserAccountMainPage));
            Routing.RegisterRoute(nameof(UserAccountPage), typeof(UserAccountPage));
            Routing.RegisterRoute(nameof(ProtocolMainPage), typeof(ProtocolMainPage));
        }
    }
}
