using Microsoft.Maui.Controls.PlatformConfiguration;

#if ANDROID
using Android.Provider;
#elif IOS || MACCATALYST
using UIKit;
#endif


namespace FireEscape.AppSettings
{
    public static class AppSettingsExtension
    {
        private const string APPLICATION_SETTINGS = "ApplicationSettings";
        private const string DROPBOX_SETTINGS = "DropboxSettings";
        private const string NEW_PROTOCOL_SETTINGS = "NewProtocolSettings";
        private const string FIREESCAPE_PROPERTIES_SETTINGS = "FireEscapePropertiesSettings";

        public static MauiAppBuilder UseAppSettings(this MauiAppBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<ApplicationSettings>(options => configuration.GetSection(APPLICATION_SETTINGS).Bind(options));
            builder.Services.Configure<DropboxSettings>(options => configuration.GetSection(DROPBOX_SETTINGS).Bind(options));
            builder.Services.Configure<NewProtocolSettings>(options => configuration.GetSection(NEW_PROTOCOL_SETTINGS).Bind(options));
            builder.Services.Configure<FireEscapePropertiesSettings>(options => configuration.GetSection(FIREESCAPE_PROPERTIES_SETTINGS).Bind(options));
            return builder;
        }

        public static string ContentFolder
        {
            get
            {
#if ANDROID
                var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
                return docsDirectory!.AbsoluteFile.Path;
#else
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
            }
        }

        public static string? DeviceIdentifier
        {
            get
            {
#if WINDOWS
                //return WindowsIdentifier.GetProcessorId();
                return null;
                
#elif ANDROID
                return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

#elif IOS || MACCATALYST
                return UIDevice.CurrentDevice.IdentifierForVendor?.ToString();
#else
                return null;
#endif
            }
        }
    }
}
