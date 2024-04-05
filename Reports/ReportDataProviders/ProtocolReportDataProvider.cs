namespace FireEscape.Reports.ReportDataProviders
{
    public class ProtocolReportDataProvider
    {
        Protocol protocol;
        ServiceabilityLimits? serviceabilityLimits;
        UserAccount userAccount;

        public ProtocolReportDataProvider(Protocol protocol, ServiceabilityLimits? serviceabilityLimits, UserAccount userAccount)
        {
            this.protocol = protocol;
            this.serviceabilityLimits = serviceabilityLimits;
            this.userAccount = userAccount;
        }

        public int ProtocolNum => protocol.ProtocolNum;
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
        public int FireEscapeNum => protocol.FireEscapeNum;

        public float? StairHeight => protocol.FireEscape.StairHeight.Value;
        public int? StairWidth => protocol.FireEscape.StairWidth.Value;

        public int? StepsCount => protocol.FireEscape.StepsCount;

        public string TestEquipment => protocol.FireEscape.FireEscapeType.BaseStairsType == BaseStairsTypeEnum.P2
            ? "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
            : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.";

        public string WeldSeamServiceability => protocol.FireEscape.WeldSeamServiceability ? "соответствует" : "не соответствует";
        public string ProtectiveServiceability => protocol.FireEscape.ProtectiveServiceability ? "соответствует" : "не соответствует";

        public string Image => protocol.HasImage ? protocol.Image! : string.Empty;

        public string Customer => protocol.Customer;

        public string UserAccountCompany => string.IsNullOrWhiteSpace(userAccount.Company) ? string.Empty : userAccount.Company;
        
        public string UserAccountSignature => string.IsNullOrWhiteSpace(userAccount.Signature) ? "___________" : userAccount.Signature;

        public List<string> GetSummary()
        {
            var summary = new List<string>();
            var fireEscape = protocol.FireEscape;

            if (!fireEscape.WeldSeamServiceability)
                summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
            if (!fireEscape.ProtectiveServiceability)
                summary.Add("конструкция не окрашена (ГОСТ 9.032)");

            if (serviceabilityLimits == null)
                return summary;

            CheckServiceability(summary, fireEscape.StairHeight,
                item => item > serviceabilityLimits.MaxStairHeight && serviceabilityLimits.MaxStairHeight > 0,
                $"высота лестницы не более {serviceabilityLimits.MaxStairHeight}м" + " ({0}м)",
                "высота лестницы не соответствует ГОСТ");

            CheckServiceability(summary, fireEscape.StairWidth,
                item => item < serviceabilityLimits.MinStairWidth,
                $"ширина лестницы не менее {serviceabilityLimits.MinStairWidth}мм" + " ({0}мм)",
                "ширина лестницы не соответствует ГОСТ");


            return summary;
        }

        private static void CheckServiceability<T>(List<string> summary, ServiceabilityProperty<T> serviceabilityProperty,
            Predicate<T?> predicate, string rejectExplanation, string defaultExplanation)
        {
            var serviceabilityType = serviceabilityProperty.Serviceability.ServiceabilityType;

            if (serviceabilityType == ServiceabilityTypeEnum.Approve)
                return;

            if (serviceabilityType == ServiceabilityTypeEnum.Reject && !string.IsNullOrWhiteSpace(serviceabilityProperty.RejectExplanation))
            {
                summary.Add(serviceabilityProperty.RejectExplanation.Replace(Environment.NewLine, " "));
                return;
            }

            if (predicate != null && predicate(serviceabilityProperty.Value))
            {
                summary.Add(string.Format(rejectExplanation, serviceabilityProperty.Value));
                return;
            }

            if (serviceabilityType == ServiceabilityTypeEnum.Reject)
                summary.Add(defaultExplanation);
        }
    }
}
