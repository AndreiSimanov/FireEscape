using FireEscape.Resources.Languages;

namespace FireEscape.Common
{
    public static class AppUtils
    {
        public static string DefaultContentFolder
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

        public static async Task<bool> IsNetworkAccess()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                return true;

            await Shell.Current.DisplayAlert(AppResources.NoConnectivity, AppResources.CheckInternetMessage, AppResources.OK);
            return false;
        }
    }
}
