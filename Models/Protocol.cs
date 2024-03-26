using FireEscape.Resources.Languages;
using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public partial class Protocol : ObservableObject
    {
        [JsonIgnore]
        [ObservableProperty]
        string? sourceFile;
        [ObservableProperty]
        DateTime created;
        [ObservableProperty]
        DateTime updated;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        string image = AppResources.NoPhoto;
        [ObservableProperty]
        uint protocolNum;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string location = string.Empty;
        [ObservableProperty]
        DateTime protocolDate;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string address = string.Empty;
        [ObservableProperty]
        uint fireEscapeNum;
        [ObservableProperty]
        string details = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FireEscapeObjectName))]
        string fireEscapeObject = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FireEscapeObjectName))]
        string customer = string.Empty;
        [ObservableProperty]
        FireEscape fireEscape = new();
        [JsonIgnore]
        public string FireEscapeObjectName => string.IsNullOrWhiteSpace(FireEscapeObject)
            ? string.IsNullOrWhiteSpace(Customer) ? AppResources.FireEscape : Customer
            : FireEscapeObject;
        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(Image) && !string.Equals(Image, AppResources.NoPhoto) && File.Exists(Image);
    }
}
