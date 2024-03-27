using Simusr2.Maui.DeviceIdentifier;

namespace FireEscape.AppSettings
{
    public static class AppSettingsExtension
    {
        private const string APPLICATION_SETTINGS = "ApplicationSettings";
        private const string FILE_HOSTING_SETTINGS = "FileHostingSettings";
        private const string NEW_PROTOCOL_SETTINGS = "NewProtocolSettings";
        private const string FIREESCAPE_PROPERTIES_SETTINGS = "FireEscapePropertiesSettings";
        private const string USER_ACCOUNT_ID = "UserAccountId";

        private static string userAccountId = string.Empty;
        public static MauiAppBuilder UseAppSettings(this MauiAppBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<ApplicationSettings>(options => configuration.GetSection(APPLICATION_SETTINGS).Bind(options));
            builder.Services.Configure<FileHostingSettings>(options => configuration.GetSection(FILE_HOSTING_SETTINGS).Bind(options));
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

        public static string UserAccountId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(userAccountId))
                    return userAccountId;

                userAccountId = Identifier.Get();
                if (string.IsNullOrWhiteSpace(userAccountId))
                {
                    userAccountId = Preferences.Get(USER_ACCOUNT_ID, string.Empty);
                    if (string.IsNullOrWhiteSpace(userAccountId))
                    {
                        userAccountId = Guid.NewGuid().ToString();
                        Preferences.Set(USER_ACCOUNT_ID, userAccountId);
                    }
                }
                return userAccountId;
            }
        }
    }
}