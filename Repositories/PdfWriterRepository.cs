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
namespace FireEscape.Repositories
{
    public class PdfWriterRepository : IReportRepository
    {
        const string FONT_NAME = "times.ttf";

        public async Task<string> CreateReportAsync(Protocol protocol, string filePath)
        {
            return await MakePdfFileAsync(protocol, filePath);
        }

        public static async Task<string> MakePdfFileAsync(Protocol protocol, string filePath)
        {
            var fontFilePath = await AddFontIfNotExisit();
            return await Task.Run(() =>
            {
                filePath = filePath + ".pdf";
                using var writer = new PdfWriter(filePath);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);
                var font = PdfFontFactory.CreateFont(fontFilePath, "Identity-H");
                document.SetFont(font);
                document.SetFontSize(12);

                MakePdfHeader(document, protocol);
                MakePdfFireEscapeOverview(document, protocol);
                MakePdfImage(pdf, document, protocol);
                MakePdfFooter(document, protocol);

                document.Close();
                return filePath;
            });
        }

        private static void MakePdfHeader(Document document, Protocol protocol)
        {
            document.Add(new Paragraph("ПРОТОКОЛ № " + protocol.ProtocolNum)
                .SetFixedLeading(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFirstLineIndent(0));

            var fireEscapeTypeDescription = protocol.FireEscape.IsEvacuation
                ? "испытания пожарной эвакуационной лестницы"
                : protocol.FireEscape.FireEscapeType.FireEscapeTypeEnum == FireEscapeTypeEnum.P2
                    ? "испытания пожарной маршевой лестницы"
                    : "испытания пожарной лестницы";

            document.Add(new Paragraph(fireEscapeTypeDescription)
                .SetFixedLeading(5)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold());

            var table = new Table(2)
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER);

            var locationCell = new Cell()
                .Add(new Paragraph(protocol.Location)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBold())
                .SetBorder(Border.NO_BORDER);

            var dateCell = new Cell()
                .Add(new Paragraph(string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBold())
                .SetBorder(Border.NO_BORDER);

            table
                .AddCell(locationCell)
                .AddCell(dateCell);
            document.Add(table);

            document.Add(new Paragraph(" "));
        }

        private static void MakePdfFireEscapeOverview(Document document, Protocol protocol)
        {
            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[]{
                    new Text("Характеристика объекта: ").SetBold(),
                    new Text((protocol.FireEscape.FireEscapeType.BaseFireEscapeTypeEnum == BaseFireEscapeTypeEnum.P2
                        ? "маршевая лестница тип П2"
                        : protocol.FireEscape.FireEscapeType.Name) + ", "),
                    new Text(protocol.FireEscape.FireEscapeMountType + " "),
                    new Text(protocol.FireEscapeObject).SetBold()})
            );

            document.Add(new Paragraph()
                .AddAll(new[] {
                    new Text(" по адресу: "),
                    new Text(protocol.FullAddress).SetBold()})
               );

            document.Add(new Paragraph("Номер испытуемого объекта: № " + protocol.FireEscapeNum)
                .SetFixedLeading(8)
                .SetBold());

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Высота лестницы: ").SetBold(),
                    new Text(protocol.FireEscape.StairHeight.ToString()).SetBold().SetUnderline(),
                    new Text(" м.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Ширина лестницы: ").SetBold(),
                    new Text(protocol.FireEscape.StairWidth.ToString()).SetBold().SetUnderline(),
                    new Text(" мм.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Количество ступеней: ").SetBold(),
                    new Text(protocol.FireEscape.StepsCount.ToString()).SetBold().SetUnderline(),
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
                    new Text(protocol.FireEscape.FireEscapeType.BaseFireEscapeTypeEnum == BaseFireEscapeTypeEnum.P2 ?
                    "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
                    : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.")}));

            document.Add(new Paragraph()
                .SetFixedLeading(12)
                .AddAll(new[] {
                    new Text("Визуальный осмотр ").SetBold(),
                    new Text("сварных швов лестниц и ограждений "),
                    new Text( protocol.FireEscape.WeldSeamQuality ? "соответствует" : "не соответствует" ).SetBold(),
                    new Text(" ГОСТ 5264 - 80. Качество защитных покрытий от коррозии  "),
                    new Text( protocol.FireEscape.ProtectiveQuality ? "соответствует" : "не соответствует" ).SetBold(),
                    new Text("  ГОСТ 9.302 - 88.")}));

            document.Add(new Paragraph(" "));
        }

        private static void MakePdfImage(PdfDocument pdf, Document document, Protocol protocol)
        {
            if (!protocol.HasImage)
                return;

            var pageSize = pdf.GetDefaultPageSize();
            var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocol.Image))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetMaxWidth(pageSize.GetWidth() / 1.5f)
                .SetMaxHeight(pageSize.GetHeight() / 1.5f)
                .SetRotationAngle(GetRotation(protocol.Image));
            document.Add(pdfImage);
            document.Add(new Paragraph(" "));
        }

        private static void MakePdfFooter(Document document, Protocol protocol)
        {
            if (string.IsNullOrWhiteSpace(protocol.Customer))
                return;

            document.Add(new Paragraph()
                .SetFixedLeading(8)
                .AddAll(new[] {
                    new Text("Присутствовали: Представитель Заказчика "),
                    new Text(protocol.Customer).SetBold()}));
            document.Add(new Paragraph("М.П.").SetFixedLeading(8));
            document.Add(new Paragraph("_______________ / ___________ /").SetTextAlignment(TextAlignment.RIGHT));
        }

        private static async Task<string> AddFontIfNotExisit()
        {
            var fontFilePath = Path.Combine(AppSettingsExtension.ContentFolder, FONT_NAME);
            if (!File.Exists(fontFilePath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(FONT_NAME);
                using var fileStream = new FileStream(fontFilePath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);
                await stream.FlushAsync();
            }
            return fontFilePath;
        }

        private static double GetRotation(string filePath)
        {
            var angle = 0;
            var orientation = ImageMetadataReader.ReadMetadata(filePath)
                .OfType<ExifIfd0Directory>()
                .FirstOrDefault()?
                .GetDescription(ExifIfd0Directory.TagOrientation);
            if (!string.IsNullOrWhiteSpace(orientation))
            {
                var angleStr = Regex.Match(orientation, @"\d+").Value;
                if (!string.IsNullOrWhiteSpace(angleStr))
                    angle = int.Parse(angleStr);
            }
            return -angle * Math.PI / 180;
        }
    }
}


