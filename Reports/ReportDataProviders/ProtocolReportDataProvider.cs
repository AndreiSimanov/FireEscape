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
                : protocol.FireEscape.FireEscapeType.FireEscapeTypeEnum == FireEscapeTypeEnum.P2
                    ? "испытания пожарной маршевой лестницы"
                    : "испытания пожарной лестницы";

        public string Location => protocol.Location;

        public string ProtocolDate => string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate);


    }
}
