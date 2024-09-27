using System.IO.Compression;

namespace FireEscape.Common;

public static class ArchiveUtils
{
    public static async Task MakeArchiveAsync(ICollection<FileInfo> files, CancellationToken ct, IProgress<double>? progress = null) //todo: move to AppUtils and rename MakeArchiveAsync
    {
        if (files.Count == 0)
            return;

        using var archiveStream = await GetArchiveStream(files, ct, progress);

        if (ct.IsCancellationRequested)
            return;

        var archiveFileName = files.FirstOrDefault()!.Directory!.Name;
        if (string.IsNullOrWhiteSpace(archiveFileName))
            archiveFileName = AppResources.Order;

        archiveFileName += archiveFileName.EndsWith('.') ? "zip" : ".zip";

        var archiveFilePath = Path.Combine(ApplicationSettings.CacheFolder, archiveFileName);

        await using (var fileStream = new FileStream(archiveFilePath, FileMode.OpenOrCreate))
        {
            await archiveStream.CopyToAsync(fileStream);
        }

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = AppResources.SharingOrderZip,
            File = new ShareFile(archiveFilePath)
        });
    }

    static async Task<MemoryStream> GetArchiveStream(ICollection<FileInfo> files, CancellationToken ct, IProgress<double>? progress = null)
    {
        var ms = new MemoryStream();
        double count = 0;
        using (var a = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var fileInfo in files)
            {
                a.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                progress?.Report(++count / files.Count);
                if (ct.IsCancellationRequested)
                    break;
                await Task.Yield();
            }
        }
        ms.Position = 0;
        return ms;
    }
}
