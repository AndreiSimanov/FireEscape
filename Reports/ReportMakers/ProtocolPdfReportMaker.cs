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

public class ProtocolPdfReportMaker(ProtocolReportDataProvider protocolRdp, ReportSettings reportSettings, string outputPath)
{
    public async Task MakeReportAsync()
    {
        var document = await PdfReportWriter.GetPdfDocumentAsync(outputPath, reportSettings.FontName, reportSettings.FontSize);
        try
        {
            MakeHeader(document);
            MakeOverview(document);
            MakeTestResultsTable(document);
            MakeImage(document);
            MakeFooter(document);
        }
        finally
        {
            document.Close();
        }
    }

    void MakeHeader(Document document)
    {
        document.Add(new Paragraph($"ПРОТОКОЛ № {protocolRdp.ProtocolNum}")
            .SetFixedLeading(5)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBold()
            .SetFirstLineIndent(0));

        document.Add(new Paragraph(protocolRdp.IsEvacuation ? 
            "испытания пожарной эвакуационной лестницы" : protocolRdp.StairsType == StairsTypeEnum.P2? 
            "испытания пожарной маршевой лестницы": 
            "испытания пожарной лестницы")
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
            .Add(new Paragraph(string.Format(reportSettings.DateFormat, protocolRdp.ProtocolDate))
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetBold())
            .SetBorder(Border.NO_BORDER);

        table
            .AddCell(locationCell)
            .AddCell(dateCell);
        document.Add(table);

        document.Add(new Paragraph(" "));
    }
    void MakeOverview(Document document)
    {
        document.Add(new Paragraph()
            .SetFixedLeading(12)
            .AddAll(new[]{
                new Text("Характеристика объекта: ").SetBold(),
                new Text(protocolRdp.StairsTypeStr + ", "),
                new Text(protocolRdp.StairsMountType + " "),
                new Text(protocolRdp.FireEscapeObject).SetBold()})
        );

        document.Add(new Paragraph()
            .AddAll(new[] {
                new Text(" по адресу: "),
                new Text(protocolRdp.FullAddress).SetBold()})
           );

        document.Add(new Paragraph($"Номер испытуемого объекта: № {protocolRdp.FireEscapeNum}")
            .SetFixedLeading(8)
            .SetBold());

        document.Add(new Paragraph()
            .SetFixedLeading(8)
            .AddAll(new[] {
                new Text("Высота лестницы: ").SetBold(),
                new Text(protocolRdp.StairsHeight.ToString(reportSettings.FloatFormat)).SetBold().SetUnderline(),
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

        document.Add(new Paragraph($"Наличие ограждения лестницы: {(protocolRdp.HasStairsFence ? "имеется" : "не имеется")} ")
                    .SetFixedLeading(8)
                    .SetBold());

        document.Add(new Paragraph()
            .SetFixedLeading(12)
            .AddAll(new[] {
                new Text("Условия проведения испытания: ").SetBold(),
                new Text("скорость ветра до 10 м/с, время суток - дневное, в условиях визуальной видимости испытателей друг друга.")}));

         document.Add(new Paragraph()
            .SetFixedLeading(12)
            .AddAll(new[] {
                new Text("Средства испытаний: ").SetBold(),
                new Text(protocolRdp.StairsType == StairsTypeEnum.P2 ?
                    "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство.":
                    "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.")}));

        document.Add(new Paragraph()
            .SetFixedLeading(12)
            .AddAll(new[] {
                new Text("Визуальный осмотр ").SetBold(),
                new Text("сварных швов лестниц и ограждений "),
                new Text( protocolRdp.WeldSeamServiceability? "соответствует" : "не соответствует").SetBold(),
                new Text(" ГОСТ 5264 - 80. Качество защитных покрытий от коррозии  "),
                new Text( protocolRdp.ProtectiveServiceability? "соответствует" : "не соответствует").SetBold(),
                new Text("  ГОСТ 9.302 - 88.")}));

        document.Add(new Paragraph(" "));
    }

    void MakeTestResultsTable(Document document)
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
        foreach (var stairsElement in protocolRdp.GetStairsElementsResult())
            MakeTestResultsRow(table, stairsElement);
        document.Add(table);
        document.Add(new Paragraph(" "));
    }

    void MakeTestResultsRow(Table table, StairsElementResult stairsElementResult)
    {
        table.StartNewRow();
        table.AddCell(MakeCell(table.GetNumberOfRows().ToString(), alignment: TextAlignment.CENTER));
        table.AddCell(MakeCell(stairsElementResult.Name));

        var testPointCount = stairsElementResult.IsAbsent ? "-" : stairsElementResult.TestPointCount.ToString();
        table.AddCell(MakeCell(testPointCount, true, TextAlignment.CENTER));

        var calcWithstandLoad = stairsElementResult.IsAbsent ? "-" : stairsElementResult.WithstandLoadCalcResult.ToString(reportSettings.FloatFormat);
        table.AddCell(MakeCell(calcWithstandLoad, true, TextAlignment.CENTER));

        var serviceability = stairsElementResult.IsAbsent ?
            "-" :
            stairsElementResult.Summary.Count > 0 ?
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

    void MakeImage(Document document)
    {
        if (!protocolRdp.HasImage)
            return;

        var pageSize = document.GetPdfDocument().GetDefaultPageSize();

        var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocolRdp.ImageFilePath))
            .SetHorizontalAlignment(HorizontalAlignment.CENTER)
            .SetMaxWidth(pageSize.GetWidth() / (100f / reportSettings.ImageScale))
            .SetMaxHeight(pageSize.GetHeight() / (100f / reportSettings.ImageScale))
            .SetRotationAngle(ImageUtils.GetRotation(protocolRdp.ImageFilePath));
        document.Add(pdfImage);
        document.Add(new Paragraph(" "));
    }

    void MakeFooter(Document document)
    {
        document.SetFontSize(10);

        var summary = protocolRdp.GetReportSummary().ToList();

        document.Add(new Paragraph()
            .SetFixedLeading(12)
            .AddAll(new[] {
                new Text("Выводы по результатам испытаний: ").SetBold(),
                new Text("В соответствии с "),
                new Text("ГОСТ Р. 53254 - 2009 ").SetBold(),
                new Text("«Техника пожарная. Лестницы пожарные наружные стационарные. Ограждения кровли. Общие технические требования. Методы испытаний» пожарная стационарная лестница, к эксплуатации "),
                new Text(summary.Count > 0 ? "не пригодна." : "пригодна.").SetBold()
            }));

        var summaryList = new List();
        foreach (var item in summary)
        {
            var listItem = new ListItem();
            listItem.Add(new Paragraph(item).SetFixedLeading(12));
            summaryList.Add(listItem);
        }
        document.Add(summaryList);

        document.Add(new Paragraph(" "));

        document.Add(new Paragraph()
            .SetFixedLeading(8)
            .AddAll(new[] {
                new Text("Испытания проводили: инженер "),
                new Text(protocolRdp.ExecutiveCompany).SetBold()}));
        document.Add(new Paragraph("М.П.").SetFixedLeading(8));
        document.Add(new Paragraph($"_______________ / {protocolRdp.PrimaryExecutorSign} /").SetTextAlignment(TextAlignment.RIGHT));
        document.Add(new Paragraph($"_______________ / {protocolRdp.SecondaryExecutorSign} /").SetTextAlignment(TextAlignment.RIGHT));

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