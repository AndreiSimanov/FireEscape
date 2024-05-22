﻿using DevExpress.Maui.Core;
using MetroLog.Maui;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace FireEscape;

public partial class App : Application
{
    public App(UserAccountService userAccountService, IOptions<ApplicationSettings> applicationSettings, ILogger<App> logger )
    {
        SetPrimaryThemeColor(applicationSettings.Value);
        SetUnitsOfMeasure(applicationSettings.Value);
        CultureInfo.CurrentCulture = SetNumberDecimalSeparator(CultureInfo.CurrentCulture);
        CultureInfo.CurrentUICulture = SetNumberDecimalSeparator(CultureInfo.CurrentUICulture);
        Localizer.StringLoader = new ResourceStringLoader(AppResources.ResourceManager);
        RemoveBorders();
        Task.Run(() => userAccountService.GetCurrentUserAccountAsync());
        InitializeComponent();
        MainPage = new AppShell();

        LogController.InitializeNavigation(page => MainPage!.Navigation.PushModalAsync(page), () => MainPage!.Navigation.PopModalAsync());
        new LogController().IsShakeEnabled = true;

        MauiExceptions.UnhandledException += (sender, args) =>
        {
            var exception = args.ExceptionObject as Exception;
            if (exception == null)
                return;
            logger.LogCritical(exception, exception.Message);
            throw exception;
        };
    }

    static CultureInfo SetNumberDecimalSeparator(CultureInfo cultureInfo)
    {
        CultureInfo result = (CultureInfo)cultureInfo.Clone();
        result.NumberFormat.NumberDecimalSeparator = ".";
        return result;
    }

    static void SetUnitsOfMeasure(ApplicationSettings applicationSettings)
    {
        UnitOfMeasureSettings.PrimaryUnitOfMeasure = applicationSettings.PrimaryUnitOfMeasure;
        UnitOfMeasureSettings.SecondaryUnitOfMeasure = applicationSettings.SecondaryUnitOfMeasure;
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
