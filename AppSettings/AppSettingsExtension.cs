namespace FireEscape.AppSettings
{
    public static class AppSettingsExtension
    {
        private const string NEW_PROTOCOL_SETTINGS = "NewProtocolSettings";
        private const string PROTOCOL_PROPERTIES_DICTIONARY = "ProtocolPropertiesDictionary";

        public static MauiAppBuilder UseAppSettings(this MauiAppBuilder builder, IConfiguration configuration)
        {
            builder.Services.Configure<NewProtocolSettings>(options => configuration.GetSection(NEW_PROTOCOL_SETTINGS).Bind(options));
            builder.Services.Configure<ProtocolPropertiesDictionary>(options => configuration.GetSection(PROTOCOL_PROPERTIES_DICTIONARY).Bind(options));
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
    }


}
