namespace FireEscape
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            /*
            var id = Preferences.Get("my_id", string.Empty);
            if (string.IsNullOrWhiteSpace(id))
            {
                id = System.Guid.NewGuid().ToString();
                Preferences.Set("my_id", id);
            }
            */
            var c = Simusr2.Maui.DeviceIdentifier.Identifier.Get();
            Console.WriteLine(c);
            MainPage = new AppShell();
        }
    }
}
