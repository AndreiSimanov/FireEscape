using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;

namespace FireEscape.Reports.ReportWriters;

public static class PdfReportWriter
{
    public static async Task<Document> GetPdfDocumentAsync(string filePath, string fontName, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        var fontFilePath = await AddFontIfNotExisitAsync(AppUtils.DefaultContentFolder, fontName);
        var pdf = new PdfDocument(new PdfWriter(filePath));
        var document = new Document(pdf);
        var font = PdfFontFactory.CreateFont(fontFilePath);
        document.SetFont(font);
        document.SetFontSize(fontSize);
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