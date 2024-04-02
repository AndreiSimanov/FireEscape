using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;

namespace FireEscape.Reports.ReportWriters
{
    internal static class PdfReportWriter
    {
        const string DEFAULT_FONT_NAME = "times.ttf";
        const float DEFAULT_FONT_SIZE = 12f;

        public static async Task<Document> GetPdfDocument(string filePath, string fontName = DEFAULT_FONT_NAME, float forntSize = DEFAULT_FONT_SIZE)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            var fontFilePath = await AddFontIfNotExisit(fontName);
            var pdf = new PdfDocument(new PdfWriter(filePath));
            var document = new Document(pdf);
            var font = PdfFontFactory.CreateFont(fontFilePath, "Identity-H");
            document.SetFont(font);
            document.SetFontSize(forntSize);
            return document;
        }

        static async Task<string> AddFontIfNotExisit(string fontName)
        {
            var fontFilePath = Path.Combine(AppSettingsExtension.ContentFolder, fontName);
            if (!File.Exists(fontFilePath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(fontName);
                using var fileStream = new FileStream(fontFilePath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);
                await stream.FlushAsync();
            }
            return fontFilePath;
        }

    }
}
