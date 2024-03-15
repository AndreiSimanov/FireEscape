using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Text.RegularExpressions;

namespace FireEscape.Common
{
    public static class PdfHelper
    {
        public static async Task MakePdfFileAsync(Protocol protocol)
        {
            string fileName = "protocol.pdf"; //todo: changt file name to the protocol attribute 

            var filePath = Path.Combine(AppSettingsExtension.ContentFolder, fileName);

            using (PdfWriter writer = new PdfWriter(filePath))
            {
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);
                Paragraph header = new Paragraph("MAUI PDF Sample")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20);

                document.Add(header);
                Paragraph subheader = new Paragraph("Welcome to .NET Multi-platform App UI")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(15);
                document.Add(subheader);
                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                if (protocol.HasImage)
                {
                    var pdfImage = new iText.Layout.Element.Image(ImageDataFactory.Create(protocol.Image))
                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                        .SetRotationAngle(GetRotation(protocol.Image));
                    document.Add(pdfImage);
                }

                Paragraph footer = new Paragraph("Don't forget to like and subscribe!")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
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
