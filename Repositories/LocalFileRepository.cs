using FireEscape.Factories;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Repositories
{
    public class LocalFileRepository : IProtocolRepository
    {
        readonly ApplicationSettings applicationSettings;
        readonly ProtocolFactory protocolFactory;


        public LocalFileRepository(IOptions<ApplicationSettings> applicationSettings, ProtocolFactory protocolFactory)
        {
            this.applicationSettings = applicationSettings.Value;
            this.protocolFactory = protocolFactory;
        }

        public async Task<Protocol> CreateProtocolAsync()
        {
            var protocol = protocolFactory.CreateDefaultProtocol();
            await SaveProtocolAsync(protocol);
            return protocol;
        }

        public async Task<Protocol> CopyProtocolAsync(Protocol protocol)
        {
            var newProtocol = protocolFactory.CopyProtocol(protocol);
            await SaveProtocolAsync(newProtocol);
            return newProtocol;
        }

        public async Task SaveProtocolAsync(Protocol protocol)
        {
            if (string.IsNullOrWhiteSpace( protocol.Id))
                protocol.Id = Path.Combine(applicationSettings.ContentFolder, Guid.NewGuid().ToString() + ".json");
            protocol.Updated = DateTime.Now;
            using var fs = File.Create(protocol.Id);
            await JsonSerializer.SerializeAsync(fs, protocol);
        }

        public async Task DeleteProtocol(Protocol protocol) =>
            await Task.Run(() =>
            {
                if (protocol.HasImage)
                    File.Delete(protocol.Image!);
                if (File.Exists(protocol.Id))
                    File.Delete(protocol.Id);
            });

        public Protocol[] GetProtocols()
        {
            var directoryInfo = new DirectoryInfo(applicationSettings.ContentFolder);
            var files = directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
            var result = new Protocol[files.Length];
            var index = 0;

            foreach (var file in files)
            {
                using var fs = File.OpenRead(file.FullName);
                Protocol? protocol = null;
                try
                {
                    protocol = JsonSerializer.Deserialize<Protocol>(fs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
                if (protocol == null)
                    protocol = protocolFactory.CreateBrokenDataProtocol(file.FullName);

                result[index] = protocol;
                index++;
            }
            return result;
        }

        public async IAsyncEnumerable<Protocol> GetProtocolsAsync()
        {
            var directoryInfo = new DirectoryInfo(applicationSettings.ContentFolder);
            var files = directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
            await Task.Yield(); // for async compatibility
            foreach (var file in files)
            {
                using var fs = File.OpenRead(file.FullName);
                Protocol? protocol = null;
                try
                {
                    // Async method works really long time cause content has small size (tested 10000 items)
                    // protocol = await JsonSerializer.DeserializeAsync<Protocol>(fs);
                    protocol = JsonSerializer.Deserialize<Protocol>(fs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }

                if (protocol == null)
                    protocol = protocolFactory.CreateBrokenDataProtocol(file.FullName);

                yield return protocol;
            }
        }

        public async Task AddPhotoAsync(Protocol protocol, FileResult? photo)
        {
            if (photo != null)
            {
                var photoFilePath = Path.Combine(applicationSettings.ContentFolder, photo.FileName);
                using var photoStream = await photo.OpenReadAsync();
                using var outputFile = File.Create(photoFilePath);
                await photoStream.CopyToAsync(outputFile);
                if (protocol.HasImage)
                    File.Delete(protocol.Image!);
                protocol.Image = photoFilePath;
                await SaveProtocolAsync(protocol);
            }
        }
    }
}
