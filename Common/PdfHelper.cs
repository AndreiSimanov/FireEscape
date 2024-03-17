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

namespace FireEscape.Common
{
    public static class PdfHelper
    {
        const string FONT_NAME = "times.ttf";
        public static async Task MakePdfFileAsync(Protocol protocol)
        {
            string fileName = "protocol.pdf"; //todo: changt file name to the protocol attribute 

            var filePath = Path.Combine(AppSettingsExtension.ContentFolder, fileName);
            var fontFilePath = await AddFontIfNotExisit();

            using (PdfWriter writer = new PdfWriter(filePath))
            {
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                var font = PdfFontFactory.CreateFont(fontFilePath, "Identity-H");
                document.SetFont(font);
                document.SetFontSize(12);

                document.Add(new Paragraph("ПРОТОКОЛ № " + protocol.ProtocolNum)
                    .SetFixedLeading(5)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFirstLineIndent(0));

                document.Add(new Paragraph(protocol.FireEscapeType.Description)
                    .SetFixedLeading(5)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold());

                Table table = new Table(2)
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

                document.Add(new Paragraph()
                    .SetFixedLeading(12)
                    .AddAll(new[]{
                        new Text("Характеристика объекта: ").SetBold(),
                        new Text((protocol.FireEscapeType.BaseFireEscapeTypeEnum == BaseFireEscapeTypeEnum.P2 ?
                            "маршевая лестница тип П2"
                            : protocol.FireEscapeType.Name) + ", "),
                        new Text(protocol.FireEscapeMountType + " "),
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
                    new Text(protocol.StairHeight.ToString()).SetBold().SetUnderline(),
                    new Text(" м.")
                }));

                document.Add(new Paragraph()
                    .SetFixedLeading(8)
                   .AddAll(new[] {
                    new Text("Ширина лестницы: ").SetBold(),
                    new Text(protocol.StairWidth.ToString()).SetBold().SetUnderline(),
                    new Text(" мм.")
                }));

                document.Add(new Paragraph()
                    .SetFixedLeading(8)
                   .AddAll(new[] {
                    new Text("Количество ступеней: ").SetBold(),
                    new Text(protocol.StepsCount.ToString()).SetBold().SetUnderline(),
                    new Text(" шт.")
                }));

                document.Add(new Paragraph()
                    .SetFixedLeading(12)
                   .AddAll(new[] {
                    new Text("Условия проведения испытания: ").SetBold(),
                    new Text("скорость ветра до 10 м/с, время суток - дневное, в условиях визуальной видимости испытателей друг друга.")
                }));

                document.Add(new Paragraph()
                    .SetFixedLeading(12)
                   .AddAll(new[] {
                    new Text("Средства испытаний: ").SetBold(),
                    new Text(protocol.FireEscapeType.BaseFireEscapeTypeEnum == BaseFireEscapeTypeEnum.P2 ?
                    "стропа металлические, лазерный дальномер, динамометр, цепь, специальное устройство."
                    : "лебёдка, динамометр, набор грузов, цепи, лазерная рулетка.")
                }));


                document.Add(new Paragraph(" "));
                if (protocol.HasImage)
                {
                    var pageSize = pdf.GetDefaultPageSize();
                    

                    var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocol.Image))
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                        .SetMaxWidth(pageSize.GetWidth()/1.5f)
                        .SetMaxHeight(pageSize.GetHeight() / 1.5f)
                        .SetRotationAngle(GetRotation(protocol.Image));
                    document.Add(pdfImage);
                }

                /*
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                Paragraph footer = new Paragraph("Don't forget to like and subscribe!")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                    .SetFontSize(14);
                document.Add(footer);
                */

                document.Close();
            }
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }

        private static async Task<string> AddFontIfNotExisit()
        {
            var fontFilePath = Path.Combine(AppSettingsExtension.ContentFolder, FONT_NAME);
            if (!File.Exists(fontFilePath))
            {
                using (var stream = await FileSystem.OpenAppPackageFileAsync(FONT_NAME))
                using (var fileStream = new FileStream(fontFilePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                    await stream.FlushAsync();
                }
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
            if (!string.IsNullOrEmpty(orientation))
            {
                var angleStr = Regex.Match(orientation, @"\d+").Value;
                if (!string.IsNullOrEmpty(angleStr))
                    angle = int.Parse(angleStr);
            }
            return -angle * Math.PI / 180;
        }
    }
}
