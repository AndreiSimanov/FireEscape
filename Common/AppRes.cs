namespace FireEscape.Common
{
    public static class AppRes
    {

        public const string NEW_PROTOCOL = "NewProtocol";
        public const string NO_PHOTO = "NoPhoto";
        public const string DELETE = "Delete";
        public const string CANCEL = "Cancel";
        public const string DELETE_PROTOCOL = "DeleteProtocol";
        public const string ERROR = "Error";
        public const string OK = "Ok";

        public static string ContentFolder => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static T? Get<T>(string resourceName)
        {
            try
            {
                var success = Application.Current!.Resources.TryGetValue(resourceName, out var outValue);

                if (success && outValue is T)
                {
                    return (T)outValue;
                }
                else
                {
                    return default;
                }
            }
            catch
            {
                return default;
            }
        }
    }
}
