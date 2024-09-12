using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace FireEscape;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    const int RequestStoragePermissionCode = 1;
    static TaskCompletionSource<bool> AllFilesAccessPermissionTCS = new();
    public static Task<bool> AllFilesAccessPermissionTask => AllFilesAccessPermissionTCS.Task;

    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            if (!Android.OS.Environment.IsExternalStorageManager)
            {
                var intent = new Intent(Android.Provider.Settings.ActionManageAppAllFilesAccessPermission);
                intent.AddCategory(Intent.CategoryDefault);
                intent.SetData(Android.Net.Uri.Parse($"package:{MauiApplication.Current.OpPackageName}"));
                Platform.CurrentActivity?.StartActivityForResult(intent, RequestStoragePermissionCode);
            }
            else
                AllFilesAccessPermissionTCS.SetResult(true);

            if (!await AllFilesAccessPermissionTask)
                Toast.MakeText(MauiApplication.Context, AppResources.FilesAccessPermissionWarning, ToastLength.Long)?.Show();
        }
        else
            AllFilesAccessPermissionTCS.SetResult(true);
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            if (requestCode == RequestStoragePermissionCode)
                AllFilesAccessPermissionTCS.SetResult(Android.OS.Environment.IsExternalStorageManager);
        }
        base.OnActivityResult(requestCode, resultCode, data);
    }
}
