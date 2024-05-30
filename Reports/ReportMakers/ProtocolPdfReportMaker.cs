using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportWriters;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using Border = iText.Layout.Borders.Border;
using Cell = iText.Layout.Element.Cell;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using TextAlignment = iText.Layout.Properties.TextAlignment;

namespace FireEscape.Reports.ReportMakers;

public static class ProtocolPdfReportMaker
{
    const string DEFAULT_FLOAT_FORMAT = "0.0";
    public static async Task MakeReportAsync(ProtocolReportDataProvider protocolRdp, string outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
            return;
        var document = await PdfReportWriter.GetPdfDocumentAsync(outputPath);
        try
        {
            MakeHeader(document, protocolRdp);
            MakeOverview(document, protocolRdp);
            MakeTestResultsTable(document, protocolRdp);
            MakeImage(document, protocolRdp);
            MakeFooter(document, protocolRdp);
        }
        finally
        {
            document.Close();
        }
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
                new Text(protocolRdp.StairsHeight.ToString(DEFAULT_FLOAT_FORMAT)).SetBold().SetUnderline(),
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

    static void MakeTestResultsTable(Document document, ProtocolReportDataProvider protocolRdp)
    {
        document.Add(new Paragraph("РЕЗУЛЬТАТЫ ИСПЫТАНИЙ")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBold()
            .SetUnderline());
        var table = new Table(5).UseAllAvailableWidth();
        table.AddHeaderCell(MakeCell("№\r\nп/п", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Наименование", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Количество\r\nточек\r\nиспытаний", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Нагрузка\r\n(кгс.)", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Результаты\r\nиспытания", alignment: TextAlignment.CENTER));
        foreach (var stairsElement in protocolRdp.GetStairsElements())
            MakeTestResultsRow(table, stairsElement);
        document.Add(table);
    }

    static void MakeTestResultsRow(Table table, StairsElementResult stairsElementResult)
    {
        table.StartNewRow();
        table.AddCell(MakeCell(table.GetNumberOfRows().ToString(), alignment: TextAlignment.CENTER));
        table.AddCell(MakeCell(stairsElementResult.Name));

        var testPointCount = stairsElementResult.TestPointCount == 0 ? "-" : stairsElementResult.TestPointCount.ToString();
        table.AddCell(MakeCell(testPointCount, true, TextAlignment.CENTER));

        var calcWithstandLoad = stairsElementResult.TestPointCount == 0 ? "-" : stairsElementResult.CalcWithstandLoad.ToString(DEFAULT_FLOAT_FORMAT);
        table.AddCell(MakeCell(calcWithstandLoad, true, TextAlignment.CENTER));

        var serviceability = stairsElementResult.TestPointCount == 0 ?
            "-" :
            stairsElementResult.Summary.Any() ?
                "Не соответствует требованиям\r\nГОСТ Р. 53254-2009\r\n" :
                "Соответствует требованиям\r\nГОСТ Р. 53254-2009\r\n";

        table.AddCell(MakeCell(serviceability, alignment: TextAlignment.CENTER));
    }

    static Cell MakeCell(string text, bool bold = false, TextAlignment? alignment = null)
    {
        var paragraph = new Paragraph(text).SetFixedLeading(12);
        if (alignment != null)
            paragraph.SetTextAlignment(alignment);
        if (bold)
            paragraph.SetBold();
        return new Cell().Add(paragraph).SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
    }


    static void MakeImage(Document document, ProtocolReportDataProvider protocolRdp)
    {
        if (!protocolRdp.HasImage)
            return;

        var pageSize = document.GetPdfDocument().GetDefaultPageSize();

        var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocolRdp.ImageFilePath))
            .SetHorizontalAlignment(HorizontalAlignment.CENTER)
            .SetMaxWidth(pageSize.GetWidth() / 1.5f)
            .SetMaxHeight(pageSize.GetHeight() / 1.5f)
            .SetRotationAngle(ImageUtils.GetRotation(protocolRdp.ImageFilePath));
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
        document.Add(new Paragraph("_______________ / " + protocolRdp.PrimaryExecutorSign + " /").SetTextAlignment(TextAlignment.RIGHT));
        document.Add(new Paragraph("_______________ / " + protocolRdp.SecondaryExecutorSign + " /").SetTextAlignment(TextAlignment.RIGHT));

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
