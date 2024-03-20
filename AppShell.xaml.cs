namespace FireEscape
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FireEscapePage), typeof(FireEscapePage));
            Routing.RegisterRoute(nameof(ProtocolPage), typeof(ProtocolPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
