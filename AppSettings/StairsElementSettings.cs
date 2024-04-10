namespace FireEscape.AppSettings
{
    public class StairsElementSettings
    {
        public StairsTypeEnum StairsType { get; set; }
        public required string Type { get; set; }
        public int Order { get; set; }
        public int TestPointCount { get; set; }
        public float WithstandLoad { get; set; }
        public bool Required { get; set; }
        public int MaxCount { get; set; }

    }
}
