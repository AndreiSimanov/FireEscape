using FireEscape.Reports.ReportDataProviders;
using FireEscape.Reports.ReportWriters;
using iText.IO.Image;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Extensions.Options;
using Border = iText.Layout.Borders.Border;
using Cell = iText.Layout.Element.Cell;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace FireEscape.Reports.ReportMakers;

public class ProtocolPdfReportMaker(IOptions<ReportSettings> reportSettings) : IProtocolPdfReportMaker
{
    ProtocolReportDataProvider? protocolRdp;
    readonly ReportSettings reportSettings = reportSettings.Value;
    public async Task MakeReportAsync(ProtocolReportDataProvider protocolRdp, string outputPath)
    {
        if (protocolRdp == null)
            throw new ArgumentNullException(nameof(protocolRdp));
        this.protocolRdp = protocolRdp;
        var tempPdfFile = outputPath + ".tmp";
        var document = await PdfReportWriter.CreatePdfDocumentAsync(tempPdfFile, reportSettings.FontName, reportSettings.FontSize);
        try
        {
            MakeHeader(document);
            MakeOverview(document);

            if (protocolRdp.StairsType == StairsTypeEnum.P2)
                MakeCalcBlockP2(document);
            else
                MakeCalcBlockP1(document);

            MakeTestResultsTable(document);
            MakeImage(document);
            MakeFooter(document);
        }
        finally
        {
            document.Close();
        }

        document = await PdfReportWriter.OpenPdfDocumentAsync(tempPdfFile, outputPath, reportSettings.FontName, reportSettings.FontSize);
        try
        {
            MakeNumberOfPages(document);
        }
        finally
        {
            document.Close();
            if (File.Exists(tempPdfFile))
                File.Delete(tempPdfFile);
        }
    }

    static Paragraph GetParagraph(string? text = null)
    {
        var paragraph = text == null ? new Paragraph() : new Paragraph(text);
        paragraph.SetFixedLeading(12).SetMargin(0);
        return paragraph;
    }

    static ListItem GetListItem(string text, string? listSymbol = null)
    {
        return GetListItem(GetParagraph(text), listSymbol);
    }

    static ListItem GetListItem(Paragraph paragraph, string? listSymbol = null)
    {
        var listItem = new ListItem();
        if (listSymbol != null)
            listItem.SetListSymbol(listSymbol);
        listItem.Add(paragraph);
        return listItem;
    }

    void MakeHeader(Document document)
    {
        document.Add(new Paragraph($"ПРОТОКОЛ № {protocolRdp!.ProtocolNum}")
            .SetFixedLeading(5)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBold()
            .SetFirstLineIndent(0));

        document.Add(new Paragraph(protocolRdp.IsEvacuation ?
            "испытания пожарной эвакуационной лестницы" : protocolRdp.StairsType == StairsTypeEnum.P2 ?
            "испытания пожарной маршевой лестницы" :
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
        document.Add(GetParagraph()
            .AddAll(new[]{
                new Text("Характеристика объекта: ").SetBold(),
                new Text(protocolRdp!.StairsTypeStr + ", "),
                new Text(protocolRdp.StairsMountType + " "),
                new Text(protocolRdp.FireEscapeObject).SetBold()})
        );

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text(" по адресу: "),
                new Text(protocolRdp.FullAddress).SetBold()})
           );

        document.Add(new Paragraph(" "));
        document.Add(GetParagraph($"Номер испытуемого объекта: № {protocolRdp.FireEscapeNum}").SetBold());

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Высота лестницы: ").SetBold(),
                new Text(protocolRdp.StairsHeight.ToString(reportSettings.FloatFormat)).SetBold().SetUnderline(),
                new Text(" м.")}));

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Ширина лестницы: ").SetBold(),
                new Text(protocolRdp.StairsWidth.ToString()).SetBold().SetUnderline(),
                new Text(" мм.")}));

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Количество ступеней: ").SetBold(),
                new Text(protocolRdp.StepsCount.ToString()).SetBold().SetUnderline(),
                new Text(" шт.")}));

        document.Add(GetParagraph($"Наличие ограждения лестницы: {(protocolRdp.HasStairsFence ? "имеется" : "не имеется")} ").SetBold());

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Условия проведения испытания: ").SetBold(),
                new Text("скорость ветра до 10 м/с, время суток - дневное, в условиях визуальной видимости испытателей друг друга.")}));

        document.Add(GetParagraph()
           .AddAll(new[] {
                new Text("Средства испытаний: ").SetBold(),
                new Text(protocolRdp.StairsType == StairsTypeEnum.P2 ?
                    "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство.":
                    "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.")}));

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Визуальный осмотр ").SetBold(),
                new Text("сварных швов лестниц и ограждений "),
                new Text( protocolRdp.WeldSeamServiceability? "соответствует" : "не соответствует").SetBold(),
                new Text(" ГОСТ 5264 - 80. Качество защитных покрытий от коррозии  "),
                new Text( protocolRdp.ProtectiveServiceability? "соответствует" : "не соответствует").SetBold(),
                new Text("  ГОСТ 9.302 - 88.")}));

        document.Add(new Paragraph(" "));
    }

    void MakeCalcBlockP1(Document document)
    {
        document.Add(GetParagraph("Расчет величины нагрузки на балку:").SetBold());
        document.Add(GetParagraph($"М = (Н*К2)/(К1*Х)*К3 = {protocolRdp!.SupportBeamsP1Calc} кгс.").SetBold());

        var platformP1Calc = protocolRdp.PlatformP1Calc;
        if (!string.IsNullOrWhiteSpace(platformP1Calc))
        {
            document.Add(GetParagraph("Расчет величины нагрузки на площадку лестницы:").SetBold());
            document.Add(GetParagraph($"Р = (S*К2)/(К4*Х)*К3 = {platformP1Calc} кгс.").SetBold());
        }

        var paramsList = new List();
        paramsList.Add(GetListItem("масса груза, при которой следует проводить испытания;", "где  М - "));
        if (!string.IsNullOrWhiteSpace(platformP1Calc))
            paramsList.Add(GetListItem("площадь площадки лестницы;", "S - "));
        paramsList.Add(GetListItem("высота лестницы, м.;", "Н - "));
        paramsList.Add(GetListItem("количество балок, при помощи которых лестница крепится к стене;", "Х - "));
        paramsList.Add(GetListItem("коэффициент, численно равный высоте расположения одного человека (пожарного) на лестнице, м, принимается равным 2,5;", "К1- "));
        paramsList.Add(GetListItem("максимальная масса одного человека (пожарного), принимается равным 120 кг.;", "К2- "));
        paramsList.Add(GetListItem("коэффициент запаса прочности, принимается равным 1,5;", "К3- "));
        if (!string.IsNullOrWhiteSpace(platformP1Calc))
            paramsList.Add(GetListItem("коэффициент, численно равный величине проекции человека на горизонталь, принимается равным 0,5;", "K4- "));
        document.Add(paramsList);
    }

    void MakeCalcBlockP2(Document document)
    {
        MakeCalcStairwayP2(document);
        MakeCalcPlatformP2(document);
    }

    void MakeCalcStairwayP2(Document document)
    {
        document.Add(GetParagraph("Расчет величины нагрузки:").SetBold());
        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Лестничный марш ").SetBold(),
                new Text("должен выдерживать испытательную нагрузку "),
                new Text("Р"),
                new Text("марш").SetFontSize(8),
                new Text(", определяемую по формуле:")
            }));

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Р").SetBold(),
                new Text("марш ").SetFontSize(8).SetBold(),
                new Text("= (L * К2) / (К4 * Х) * К3 * cos ").SetBold(),
                new Text("α,     (1)")}));

        var stairwayP2Lens = new List<Text>();
        foreach (var stairwayP2Len in protocolRdp!.StairwayP2Lens)
        {
            stairwayP2Lens.Add(new Text("L"));
            stairwayP2Lens.Add(new Text(stairwayP2Len.Item1).SetFontSize(8));
            stairwayP2Lens.Add(new Text($"={stairwayP2Len.Item2} м; "));
        }
        document.Add(GetParagraph().AddAll(stairwayP2Lens).SetBold());

        foreach (var stairwayP2Calc in protocolRdp!.StairwayP2Calc)
            document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Р"),
                new Text($"марш.{stairwayP2Calc.Item1}").SetFontSize(8),
                new Text($"= {stairwayP2Calc.Item2} кгс.")}).SetBold());

        var paramsList = new List();
        paramsList.Add(GetListItem("длина марша лестницы, м;", "где  L - "));
        paramsList.Add(GetListItem("максимальная нагрузка, создаваемая одним человеком (пожарным), принимается равной 1,2 кН (120 кгс);", "K2- "));
        paramsList.Add(GetListItem("коэффициент запаса прочности, принимается равным 1,5;", "K3- "));
        paramsList.Add(GetListItem("коэффициент, численно равный величине проекции человека на горизонталь, м, принимается равным 0,5;", "K4- "));
        paramsList.Add(GetListItem("количество балок, при помощи которых лестница крепится к стене, шт.;", "Х - "));

        var p = GetParagraph()
            .AddAll(new[] {
                new Text("угол наклона плоскости лестницы к горизонтали (α = 45"),
                new Text("0").SetFontSize(8).SetTextRise(5),
                new Text(");")});
        paramsList.Add(GetListItem(p, "α  - "));
        document.Add(paramsList);
        document.Add(new Paragraph(" "));
    }

    void MakeCalcPlatformP2(Document document)
    {
        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Площадка лестницы ").SetBold(),
                new Text("должна выдерживать испытательную нагрузку "),
                new Text("Р"),
                new Text("площ").SetFontSize(8),
                new Text(", определяемую по формуле:")
            }));

        document.Add(GetParagraph()
                   .AddAll(new[] {
                new Text("Р").SetBold(),
                new Text("площ ").SetFontSize(8).SetBold(),
                new Text("= (S * К2) / (К4 * Х) * К3").SetBold(),
                new Text(",     (2)")}));

        var platformP2Sizes = new List<Text>();
        foreach (var platformP2Size in protocolRdp!.PlatformP2Sizes)
        {
            platformP2Sizes.Add(new Text("S"));
            platformP2Sizes.Add(new Text(platformP2Size.Item1).SetFontSize(8));
            platformP2Sizes.Add(new Text($"={platformP2Size.Item2} м"));
            platformP2Sizes.Add(new Text("2").SetFontSize(8).SetTextRise(5));
            platformP2Sizes.Add(new Text("; "));
        }
        document.Add(GetParagraph().AddAll(platformP2Sizes).SetBold());

        foreach (var stairwayP2Calc in protocolRdp!.PlatformP2Calc)
            document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Р"),
                new Text($"площ.{stairwayP2Calc.Item1}").SetFontSize(8),
                new Text($"= {stairwayP2Calc.Item2} кгс.")}).SetBold());

        var paramsList = new List();
        paramsList.Add(GetListItem("площадь площадки лестницы;", "где  S - "));
        paramsList.Add(GetListItem("максимальная нагрузка, создаваемая одним человеком (пожарным), принимается равной 1,2 кН (120 кгс);", "K2- "));
        paramsList.Add(GetListItem("коэффициент запаса прочности, принимается равным 1,5;", "K3- "));
        paramsList.Add(GetListItem("коэффициент, численно равный величине проекции человека на горизонталь, м, принимается равным 0,5;", "K4- "));
        paramsList.Add(GetListItem("количество балок, при помощи которых лестница крепится к стене, шт.;", "Х - "));
        document.Add(paramsList);
    }

    void MakeTestResultsTable(Document document)
    {
        document.Add(new Paragraph("РЕЗУЛЬТАТЫ НАГРУЗОЧНЫХ ИСПЫТАНИЙ")
            .SetKeepWithNext(true)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetBold()
            .SetUnderline());
        var table = new Table(5).UseAllAvailableWidth();
        table.AddHeaderCell(MakeCell("№\r\nп/п", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Наименование", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Количество\r\nточек\r\nиспытаний", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Нагрузка\r\n(кгс.)", alignment: TextAlignment.CENTER));
        table.AddHeaderCell(MakeCell("Результаты\r\nиспытания", alignment: TextAlignment.CENTER));
        foreach (var stairsElement in protocolRdp!.StairsElementsResult)
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
            stairsElementResult.IsDeformation ?
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
        return new Cell().Add(paragraph).SetVerticalAlignment(VerticalAlignment.MIDDLE).SetKeepTogether(true);
    }

    void MakeImage(Document document)
    {
        if (!protocolRdp!.HasImage)
            return;

        var pageSize = document.GetPdfDocument().GetDefaultPageSize();

        var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocolRdp.ImageFilePath))
            .SetHorizontalAlignment(HorizontalAlignment.CENTER)
            .SetMaxWidth(pageSize.GetWidth() / (100f / reportSettings.ImageScale))
            .SetMaxHeight(pageSize.GetHeight() / (100f / reportSettings.ImageScale))
            .SetRotationAngle(ImageUtils.GetRotation(protocolRdp.ImageFilePath));
        document.Add(new Paragraph(" "));
        document.Add(pdfImage);
        document.Add(new Paragraph(" "));
    }

    void MakeFooter(Document document)
    {
        document.Add(new AreaBreak());

        var summary = new List<string>();
        if (!protocolRdp!.WeldSeamServiceability)
            summary.Add("сварные швы не соответствуют (ГОСТ 5264)");
        if (!protocolRdp.ProtectiveServiceability)
            summary.Add("конструкция не окрашена (ГОСТ 9.032)");

        if (protocolRdp.StairsType == StairsTypeEnum.P2 && !protocolRdp.HasStairsFence)
            summary.Add($"Нет ограждения лестницы");

        if (protocolRdp.StairsType == StairsTypeEnum.P1_2 && !protocolRdp.HasStairsFence)
        {
            var maxStairsP1Height = protocolRdp.MaxStairsP1Height;
            if (maxStairsP1Height.HasValue)
            { 
                if (protocolRdp.StairsHeight > maxStairsP1Height)
                    summary.Add($"Высота лестницы более {maxStairsP1Height} метров (нет ограждения лестницы)");
            }
            else
                summary.Add($"Нет ограждения лестницы");
        }

        summary.AddRange(protocolRdp!.ReportSummary);

        document.Add(GetParagraph()
            .SetFontSize(10)
            .AddAll(new[] {
                new Text("Выводы по результатам испытаний: ").SetBold(),
                new Text("В соответствии с "),
                new Text("ГОСТ Р. 53254 - 2009 ").SetBold(),
                new Text("«Техника пожарная. Лестницы пожарные наружные стационарные. Ограждения кровли. Общие технические требования. Методы испытаний» пожарная стационарная лестница, к эксплуатации "),
                new Text(summary.Count > 0 ? "не пригодна." : "пригодна.").SetBold()
            }));

        var summaryList = new List();
        summary.ForEach(item => summaryList.Add(GetListItem(FirstCharToUpper(item))));
        summaryList.SetFontSize(10);
        document.Add(summaryList);

        document.Add(new Paragraph(" "));

        document.Add(GetParagraph()
            .AddAll(new[] {
                new Text("Испытания проводили: инженер "),
                new Text(protocolRdp.ExecutiveCompany).SetBold()}));
        document.Add(GetParagraph("М.П."));
        document.Add(new Paragraph($"_______________ / {protocolRdp.PrimaryExecutorSign} /").SetTextAlignment(TextAlignment.RIGHT));
        document.Add(new Paragraph($"_______________ / {protocolRdp.SecondaryExecutorSign} /").SetTextAlignment(TextAlignment.RIGHT));

        if (!string.IsNullOrWhiteSpace(protocolRdp.Customer))
        {
            document.Add(new Paragraph(" "));
            document.Add(GetParagraph()
                .AddAll(new[] {
                new Text("Присутствовали: Представитель Заказчика "),
                new Text(protocolRdp.Customer).SetBold()}));
            document.Add(GetParagraph("М.П."));
            document.Add(new Paragraph($"_______________ / {protocolRdp.CustomerSign} /").SetTextAlignment(TextAlignment.RIGHT));
        }
    }

    static void MakeNumberOfPages(Document document)
    {
        int numberOfPages = document.GetPdfDocument().GetNumberOfPages();
        for (int i = 1; i <= numberOfPages; i++)
        {
            document.ShowTextAligned(GetParagraph("стр. " + i + " из " + numberOfPages).
                SetFontSize(10).SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY),
                559, 17, i, TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);
        }
    }
    static string FirstCharToUpper(string input) =>
       input switch
       {
           null => throw new ArgumentNullException(nameof(input)),
           "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
           _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
       };
}