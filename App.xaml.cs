using DevExpress.Maui.Core;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace FireEscape
{
    public partial class App : Application
    {
        public App(IOptions<ApplicationSettings> applicationSettings)
        {
            SetPrimaryThemeColor(applicationSettings.Value);

            CultureInfo.CurrentCulture = SetNumberDecimalSeparator(CultureInfo.CurrentCulture);
            CultureInfo.CurrentUICulture = SetNumberDecimalSeparator(CultureInfo.CurrentUICulture);
            Localizer.StringLoader = new ResourceStringLoader(FireEscape.Resources.Languages.AppResources.ResourceManager);
            
            InitializeComponent();
            MainPage = new AppShell();
        }

        static CultureInfo SetNumberDecimalSeparator(CultureInfo cultureInfo)
        {
            CultureInfo result = (CultureInfo)cultureInfo.Clone();
            result.NumberFormat.NumberDecimalSeparator = ".";
            return result;
        }

        static void SetPrimaryThemeColor(ApplicationSettings applicationSettings)
        {
            ThemeManager.UseAndroidSystemColor = false;
            ThemeManager.ApplyThemeToSystemBars = true;
            ThemeManager.Theme = new Theme(Color.FromArgb(applicationSettings.PrimaryThemeColor));
        }
    }
}
