﻿using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportWriters;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using Border = iText.Layout.Borders.Border;
using Cell = iText.Layout.Element.Cell;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace FireEscape.Reports.ReportMakers
{
    public static class ProtocolPdfReportMaker
    {
        public static async Task<string> MakeReportAsync(ProtocolReportDataProvider protocolRdp, string filePath)
        {
            var document = await PdfReportWriter.GetPdfDocumentAsync(filePath);
            try
            {
                MakeHeader(document, protocolRdp);
                MakeOverview(document, protocolRdp);
                MakeImage(document, protocolRdp);
                MakeFooter(document, protocolRdp);
            }
            finally
            {
                document.Close();
            }
            return filePath;
        }

        static void MakeHeader(Document document, ProtocolReportDataProvider protocolRdp)
        {
            document.Add(new Paragraph("ПРОТОКОЛ № " + protocolRdp.ProtocolNum)
                .SetFixedLeading(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFirstLineIndent(0));

            document.Add(new Paragraph(protocolRdp.StairsTypeDescription)
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
        static void MakeOverview(Document document, ProtocolReportDataProvider protocolRdp)
        {
            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[]{
                    new Text("Характеристика объекта: ").SetBold(),
                    new Text(protocolRdp.StairsType + ", "),
                    new Text(protocolRdp.StairsMountType + " "),
                    new Text(protocolRdp.FireEscapeObject).SetBold()})
            );

            document.Add(new Paragraph()
                .AddAll(new[] {
                    new Text(" по адресу: "),
                    new Text(protocolRdp.FullAddress).SetBold()})
               );

            document.Add(new Paragraph("Номер испытуемого объекта: № " + protocolRdp.FireEscapeNum)
                .SetFixedLeading(8)
                .SetBold());

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Высота лестницы: ").SetBold(),
                    new Text(protocolRdp.StairsHeight.ToString()).SetBold().SetUnderline(),
                    new Text(" м.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Ширина лестницы: ").SetBold(),
                    new Text(protocolRdp.StairsWidth.ToString()).SetBold().SetUnderline(),
                    new Text(" мм.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Количество ступеней: ").SetBold(),
                    new Text(protocolRdp.StepsCount.ToString()).SetBold().SetUnderline(),
                    new Text(" шт.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[] {
                    new Text("Условия проведения испытания: ").SetBold(),
                    new Text("скорость ветра до 10 м/с, время суток - дневное, в условиях визуальной видимости испытателей друг друга.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[] {
                    new Text("Средства испытаний: ").SetBold(),
                    new Text(protocolRdp.TestEquipment)}));

            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[] {
                    new Text("Визуальный осмотр ").SetBold(),
                    new Text("сварных швов лестниц и ограждений "),
                    new Text( protocolRdp.WeldSeamServiceability).SetBold(),
                    new Text(" ГОСТ 5264 - 80. Качество защитных покрытий от коррозии  "),
                    new Text( protocolRdp.ProtectiveServiceability).SetBold(),
                    new Text("  ГОСТ 9.302 - 88.")}));

            document.Add(new Paragraph(" "));
        }

        static void MakeImage(Document document, ProtocolReportDataProvider protocolRdp)
        {
            if (!protocolRdp.HasImage)
                return;

            var pageSize = document.GetPdfDocument().GetDefaultPageSize();

            var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocolRdp.Image))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetMaxWidth(pageSize.GetWidth() / 1.5f)
                .SetMaxHeight(pageSize.GetHeight() / 1.5f)
                .SetRotationAngle(ImageUtils.GetRotation(protocolRdp.Image));
            document.Add(pdfImage);
            document.Add(new Paragraph(" "));
        }

        static void MakeFooter(Document document, ProtocolReportDataProvider protocolRdp)
        {
            document.SetFontSize(10);

            var summary = protocolRdp.GetSummary();

            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[] {
                    new Text("Выводы по результатам испытаний: ").SetBold(),
                    new Text("В соответствии с "),
                    new Text("ГОСТ Р. 53254 - 2009 ").SetBold(),
                    new Text("«Техника пожарная. Лестницы пожарные наружные стационарные. Ограждения кровли. Общие технические требования. Методы испытаний» пожарная стационарная лестница, к эксплуатации "),
                    new Text(summary.Any()? "не пригодна." : "пригодна.").SetBold()
                }));
            foreach (var item in summary)
                document.Add(new Paragraph("- " + item).SetFixedLeading(8));
            document.Add(new Paragraph(" "));

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Испытания проводили: инженер "),
                    new Text(protocolRdp.ExecutiveCompany).SetBold()}));
            document.Add(new Paragraph("М.П.").SetFixedLeading(8));
            document.Add(new Paragraph("_______________ / " + protocolRdp.UserAccountSignature + " /").SetTextAlignment(TextAlignment.RIGHT));
            document.Add(new Paragraph("_______________ / ___________ /").SetTextAlignment(TextAlignment.RIGHT));

            if (!string.IsNullOrWhiteSpace(protocolRdp.Customer))
            {
                document.Add(new Paragraph()
                    .SetFixedLeading(8)
                    .AddAll(new[] {
                    new Text("Присутствовали: Представитель Заказчика "),
                    new Text(protocolRdp.Customer).SetBold()}));
                document.Add(new Paragraph("М.П.").SetFixedLeading(8));
                document.Add(new Paragraph("_______________ / ___________ /").SetTextAlignment(TextAlignment.RIGHT));
            }
        }
    }
}
