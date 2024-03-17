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
        //const string FONT_NAME = "OpenSans-Regular.ttf";
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

                Paragraph header = new Paragraph("ПРОТОКОЛ № " + protocol.ProtocolNum)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFontSize(12);

                document.Add(header);

                Paragraph subheader = new Paragraph(protocol.FireEscapeType.Description)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetFontSize(12);

                document.Add(subheader);

                Paragraph pLocation = new Paragraph(protocol.Location)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetBold()
                    .SetFontSize(12);
                Paragraph pDate = new Paragraph(string.Format("{0:“dd” MMMM yyyy г.}", protocol.ProtocolDate)  )
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBold()
                    .SetFontSize(12);


                Table table = new Table(2)
                    .UseAllAvailableWidth()
                    .SetBorder(Border.NO_BORDER);

                var locationCell = new Cell().Add(pLocation);
                locationCell.SetBorder(Border.NO_BORDER);
                var dateCell = new Cell().Add(pDate);
                dateCell.SetBorder(Border.NO_BORDER);
                table.AddCell(locationCell);
                table.AddCell(dateCell);
                document.Add(table);

                document.Add(new Paragraph(" "));
                





                var t = new Text("Характеристика объекта: ").SetBold();

                var t1 = new Text((protocol.FireEscapeType.BaseFireEscapeTypeEnum == BaseFireEscapeTypeEnum.P2 ? 
                    "маршевая лестница тип П2" 
                    : protocol.FireEscapeType.Name) + ", ");
                var t2 = new Text(protocol.FireEscapeMountType + " ");
                var t3 = new Text(protocol.FireEscapeObject).SetBold();
                var t4 = new Text(" по адресу: ");
                var t5 = new Text(protocol.FullAddress).SetBold();
                Paragraph p = new Paragraph().SetFontSize(12);
                p.AddAll(new []{t, t1, t2, t3});
                document.Add(p);

                p = new Paragraph().SetFontSize(12);
                p.AddAll(new[] { t4, t5 });
                document.Add(p);

                document.Add(new Paragraph(" "));


                document.Add(new Paragraph("Номер испытуемого объекта: № " + protocol.FireEscapeNum)
                    .SetBold()
                    .SetFontSize(12));

                 

                //LineSeparator ls = new LineSeparator(new SolidLine());
                //document.Add(ls);



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

                Paragraph footer = new Paragraph("Don't forget to like and subscribe!")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                    .SetFontSize(14);

                document.Add(footer);
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
