﻿namespace FireEscape
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(ProtocolDetailsPage), typeof(ProtocolDetailsPage));
        }
    }
}
