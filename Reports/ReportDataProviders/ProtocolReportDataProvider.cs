namespace FireEscape.Reports.ReportDataProviders
{
    internal class ProtocolReportDataProvider
    {
        Protocol protocol;
        UserAccount userAccount;

        public ProtocolReportDataProvider(Protocol protocol, UserAccount userAccount)
        {
            this.protocol = protocol;
            this.userAccount = userAccount;
        }

        public uint ProtocolNum => protocol.ProtocolNum;
        public string FireEscapeTypeDescription => protocol.FireEscape.IsEvacuation
            ? "испытания пожарной эвакуационной лестницы"
            : protocol.FireEscape.FireEscapeType.StairsType == StairsTypeEnum.P2
                ? "испытания пожарной маршевой лестницы"
                : "испытания пожарной лестницы";

        public string Location => protocol.Location;

        public string ProtocolDate => string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate);

        public string FireEscapeType => protocol.FireEscape.FireEscapeType.BaseStairsType == BaseStairsTypeEnum.P2
            ? "маршевая лестница тип П2"
            : protocol.FireEscape.FireEscapeType.Name;

        public string FireEscapeMountType => protocol.FireEscape.FireEscapeMountType;

        public string FireEscapeObject => protocol.FireEscapeObject;
        public string FullAddress => protocol.FullAddress;
        public uint FireEscapeNum => protocol.FireEscapeNum;

        public float? StairHeight => protocol.FireEscape.StairHeight;
        public uint? StairWidth => protocol.FireEscape.StairWidth;

        public uint? StepsCount => protocol.FireEscape.StepsCount;

        public string TestEquipment => protocol.FireEscape.FireEscapeType.BaseStairsType == BaseStairsTypeEnum.P2
            ? "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
            : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.";

        public string WeldSeamQuality => protocol.FireEscape.WeldSeamQuality ? "соответствует" : "не соответствует";
        public string ProtectiveQuality => protocol.FireEscape.ProtectiveQuality ? "соответствует" : "не соответствует";

        public string Image => protocol.HasImage ? protocol.Image! : string.Empty;

        public string Customer => protocol.Customer;

        public string UserAccountCompany => string.IsNullOrWhiteSpace(userAccount.Company) ? string.Empty : userAccount.Company;
        
        public string UserAccountSignature => string.IsNullOrWhiteSpace(userAccount.Signature) ? "___________" : userAccount.Signature;



    }
}
