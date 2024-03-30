using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportWriters;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Text.RegularExpressions;
using iText.Kernel.Font;
using Cell = iText.Layout.Element.Cell;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using Border = iText.Layout.Borders.Border;


namespace FireEscape.Reports.ReportMakers
{
    internal static class ProtocolPdfReportMaker
    {
        public static async Task<string> MakeReport(ProtocolReportDataProvider protocolRdp, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));
            filePath = filePath + ".pdf";
            using var writer = new PdfReportWriter(filePath);
            var document = await writer.GetPdfDocument();

            MakePdfHeader(document, protocolRdp);
            MakePdfFireEscapeOverview(document, protocolRdp);
            MakePdfImage(document, protocolRdp);
            MakePdfFooter(document, protocolRdp);

            document.Close();
            return filePath;
        }

        private static void MakePdfHeader(Document document, ProtocolReportDataProvider protocolRdp)
        {
            document.Add(new Paragraph("ПРОТОКОЛ № " + protocolRdp.ProtocolNum)
                .SetFixedLeading(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFirstLineIndent(0));

            document.Add(new Paragraph(protocolRdp.FireEscapeTypeDescription)
                .SetFixedLeading(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            var table = new Table(2)
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);

            var locationCell = new Cell()
                .Add(new Paragraph(protocolRdp.Location)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBold())
                .SetBorder(Border.NO_BORDER);

            var dateCell = new Cell()
                .Add(new Paragraph(protocolRdp.ProtocolDate)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBold())
                .SetBorder(Border.NO_BORDER);

            table
                .AddCell(locationCell)
                .AddCell(dateCell);
            document.Add(table);

            document.Add(new Paragraph(" "));
        }
        private static void MakePdfFireEscapeOverview(Document document, ProtocolReportDataProvider protocolRdp)
        {

        }

        private static void MakePdfImage(Document document, ProtocolReportDataProvider protocolRdp)
        {

        }

        private static void MakePdfFooter(Document document, ProtocolReportDataProvider protocolRdp)
        {

        }

    }
}
