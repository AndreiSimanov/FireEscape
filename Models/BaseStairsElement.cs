using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public abstract partial class BaseStairsElement : ObservableObject
    {
        [JsonIgnore]
        public string Name => GetName();
        [JsonIgnore]
        public StairsTypeEnum StairsType => GetStairsType();
        [ObservableProperty]
        int testPointCount;
        [ObservableProperty]
        float withstandLoad; 
        [ObservableProperty]
        bool serviceability;
        [ObservableProperty]
        string rejectExplanation = string.Empty;

        protected abstract string GetName();
        protected abstract StairsTypeEnum GetStairsType();
    }
}
