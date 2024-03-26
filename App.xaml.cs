using DevExpress.Maui.Core;
using System.Globalization;

namespace FireEscape
{
    public partial class App : Application
    {
        public App()
        {
            CultureInfo.CurrentCulture = SetNumberDecimalSeparator(CultureInfo.CurrentCulture);
            CultureInfo.CurrentUICulture = SetNumberDecimalSeparator(CultureInfo.CurrentUICulture);
            Localizer.StringLoader = new ResourceStringLoader(FireEscape.Resources.Languages.AppResources.ResourceManager);
            InitializeComponent();
            MainPage = new AppShell();
        }

        private static CultureInfo SetNumberDecimalSeparator(CultureInfo cultureInfo)
        {
            CultureInfo result = (CultureInfo)cultureInfo.Clone();
            result.NumberFormat.NumberDecimalSeparator = ".";
            return result;
        }
    }
}
