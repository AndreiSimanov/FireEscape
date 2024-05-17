using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using static Dropbox.Api.TeamLog.SharedLinkAccessLevel;

namespace FireEscape.Reports.ReportWriters;

public static class PdfReportWriter
{
    const string DEFAULT_FONT_NAME = "times.ttf";
    const float DEFAULT_FONT_SIZE = 12f;

    public static async Task<Document> GetPdfDocumentAsync(string filePath, string fontName = DEFAULT_FONT_NAME, float forntSize = DEFAULT_FONT_SIZE)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        var fontFilePath = await AddFontIfNotExisitAsync(Path.GetDirectoryName(filePath)!, fontName);
        var pdf = new PdfDocument(new PdfWriter(filePath));
        var document = new Document(pdf);
        var font = PdfFontFactory.CreateFont(fontFilePath);
        document.SetFont(font);
        document.SetFontSize(forntSize);
        document.SetCharacterSpacing(.2f);
        return document;
    }

    static async Task<string> AddFontIfNotExisitAsync(string filePath, string fontName)
    {
        var fontFilePath = Path.Combine(filePath, fontName);
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
