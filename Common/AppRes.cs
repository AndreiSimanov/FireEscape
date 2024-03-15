namespace FireEscape.Common
{
    public static class AppRes
    {

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
