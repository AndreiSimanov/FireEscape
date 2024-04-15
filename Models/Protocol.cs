using FireEscape.Resources.Languages;
using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public partial class Protocol : ObservableObject, ICloneable
    {
        public const string NO_PHOTO = "nophoto.svg";

        [ObservableProperty]
        string? id;
        [ObservableProperty]
        DateTime created;
        [ObservableProperty]
        DateTime updated;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasImage))]
        string? image;
        [ObservableProperty]
        int protocolNum;
        [ObservableProperty]
        DateTime protocolDate;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string location = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullAddress))]
        string address = string.Empty;
        [ObservableProperty]
        int fireEscapeNum;
        [ObservableProperty]
        string details = string.Empty;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FireEscapeObjectName))]
        string fireEscapeObject = string.Empty;
        [ObservableProperty]
        Stairs stairs = new();
        /*
        [JsonIgnore]//!!
        public string FireEscapeObjectName => string.IsNullOrWhiteSpace(FireEscapeObject)
            ? string.IsNullOrWhiteSpace(Customer) ? AppResources.FireEscape : Customer : FireEscapeObject;
        */
        [JsonIgnore]
        public string FireEscapeObjectName => FireEscapeObject;
        [JsonIgnore]
        public string FullAddress => Location + ", " + Address;
        [JsonIgnore]
        public bool HasImage => !string.IsNullOrWhiteSpace(Image) && !string.Equals(Image, NO_PHOTO) && File.Exists(Image);

        public object Clone() => MemberwiseClone();
    }
}
