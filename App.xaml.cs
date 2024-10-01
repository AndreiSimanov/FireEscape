using DevExpress.Maui.Core;
using MetroLog.Maui;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace FireEscape;

public partial class App : Application
{
    readonly ApplicationSettings applicationSettings;
    public App(IUserAccountService userAccountService, IOptions<ApplicationSettings> applicationSettings, ILogger<App> logger)
    {
        this.applicationSettings = applicationSettings.Value;
        SetThemeColor();
        CultureInfo.CurrentCulture = SetNumberDecimalSeparator(CultureInfo.CurrentCulture);
        CultureInfo.CurrentUICulture = SetNumberDecimalSeparator(CultureInfo.CurrentUICulture);
        Localizer.StringLoader = new ResourceStringLoader(AppResources.ResourceManager);
        RemoveBorders();
        userAccountService.GetCurrentUserAccountAsync().SafeFireAndForget(ex => logger.LogError(ex, ex.Message));
        InitializeComponent();
        MainPage = new AppShell();

        LogController.InitializeNavigation(page => MainPage!.Navigation.PushModalAsync(page), () => MainPage!.Navigation.PopModalAsync());
        new LogController().IsShakeEnabled = this.applicationSettings.LogPageShakeEnabled;

        MauiExceptions.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is not Exception exception)
                return;
            logger.LogCritical(exception, message: exception.Message);
            throw exception;
        };
    }

    static CultureInfo SetNumberDecimalSeparator(CultureInfo cultureInfo)
    {
        CultureInfo result = (CultureInfo)cultureInfo.Clone();
        result.NumberFormat.NumberDecimalSeparator = ".";
        return result;
    }

    void SetThemeColor()
    {
        ThemeManager.UseAndroidSystemColor = false;
        ThemeManager.ApplyThemeToSystemBars = true;
        ThemeManager.ThemeChanged += ThemeChanged;
        ThemeManager.Theme = new Theme(Color.FromArgb(ThemeManager.IsLightTheme ? applicationSettings.LightThemeColor : applicationSettings.DarkThemeColor));
        ColorSettings.SetColors(ThemeManager.IsLightTheme ? applicationSettings.LightColorSettings : applicationSettings.DarkColorSettings); 
    }

    void ThemeChanged(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeChanged;
        ThemeManager.Theme = new Theme(Color.FromArgb(ThemeManager.IsLightTheme ? applicationSettings.LightThemeColor : applicationSettings.DarkThemeColor));
        ColorSettings.SetColors(ThemeManager.IsLightTheme ? applicationSettings.LightColorSettings : applicationSettings.DarkColorSettings);
        ThemeManager.ThemeChanged += ThemeChanged;
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
