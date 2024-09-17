using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;

namespace FireEscape.Reports.ReportWriters;

public static class PdfReportWriter
{
    public static Task<Document> CreatePdfDocumentAsync(string filePath, string fontName, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        var pdfDoc = new PdfDocument(new PdfWriter(filePath));
        return GetDocument(pdfDoc, fontName, fontSize);
    }

    public static Task<Document> OpenPdfDocumentAsync(string sourceFilePath, string destFilePath, string fontName, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
            throw new ArgumentNullException(nameof(sourceFilePath));
        if (string.IsNullOrWhiteSpace(destFilePath))
            throw new ArgumentNullException(nameof(destFilePath));

        var pdfDoc = new PdfDocument(new PdfReader(sourceFilePath), new PdfWriter(destFilePath));
        return GetDocument(pdfDoc, fontName, fontSize);
    }

    static async Task<Document> GetDocument(PdfDocument pdfDoc, string fontName, float fontSize)
    {
        var fontFilePath = await AddFontIfNotExisitAsync(await ApplicationSettings.GetDefaultContentFolderAsync(), fontName);
        var document = new Document(pdfDoc);
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