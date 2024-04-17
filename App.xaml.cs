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
            RemoveBorders();
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

        public static void RemoveBorders()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });
        }

    }
}
