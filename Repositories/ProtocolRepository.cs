using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Repositories
{
    public class ProtocolRepository : IProtocolRepository
    {
        readonly NewProtocolSettings newProtocolSettings;
        readonly FireEscapePropertiesSettings FireEscapePropertiesSettings;

        public ProtocolRepository(IOptions<NewProtocolSettings> newProtocolSettings, IOptions<FireEscapePropertiesSettings> FireEscapePropertiesSettings)
        {
            this.newProtocolSettings = newProtocolSettings.Value;
            this.FireEscapePropertiesSettings = FireEscapePropertiesSettings.Value;
        }

        public async Task<Protocol> CreateProtocolAsync()
        {
            var protocol = CreateDefaultProtocol();
            await SaveProtocolAsync(protocol);
            return protocol;
        }

        public async Task SaveProtocolAsync(Protocol protocol)
        {
            var filePath = protocol.SourceFile ?? Path.Combine(AppSettingsExtension.ContentFolder, Guid.NewGuid().ToString() + ".json");
            protocol.Updated = DateTime.Now;
            using var fs = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fs, protocol);
            protocol.SourceFile = filePath;
        }

        public async Task DeleteProtocol(Protocol protocol)
        {
            await Task.Run(() =>
            {
                if (protocol.HasImage)
                    File.Delete(protocol.Image);
                if (File.Exists(protocol.SourceFile))
                    File.Delete(protocol.SourceFile);
            });
        }

        public async IAsyncEnumerable<Protocol> GetProtocolsAsync()
        {
            var files = Directory.GetFiles(AppSettingsExtension.ContentFolder, "*.json");
            foreach (var file in files)
            {
                using var fs = File.OpenRead(file);
                Protocol? protocol = null;
                try
                {
                    protocol = await JsonSerializer.DeserializeAsync<Protocol>(fs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");

                    protocol = CreateDefaultProtocol();
                    protocol.FireEscapeObject = AppResources.BrokenData;
                    protocol.SourceFile = file;
                }

                if (protocol != null)
                {
                    protocol.SourceFile = file;
                    yield return protocol;
                }
            }
        }

        public async Task AddProtocolPhotoAsync(Protocol protocol, FileResult? photo)
        {
            if (photo != null)
            {
                var photoFilePath = Path.Combine(AppSettingsExtension.ContentFolder, photo.FileName);
                using var photoStream = await photo.OpenReadAsync();
                using var outputFile = File.Create(photoFilePath);
                await photoStream.CopyToAsync(outputFile);

                if (protocol.HasImage)
                    File.Delete(protocol.Image);

                protocol.Image = photoFilePath;
            }
        }

        private Protocol CreateDefaultProtocol()
        {
            return new Protocol()
            {
                Image = AppResources.NoPhoto!,
                ProtocolNum = newProtocolSettings.ProtocolNum,
                Location = newProtocolSettings.Location,
                ProtocolDate = DateTime.Today,
                FireEscapeNum = newProtocolSettings.FireEscapeNum,
                FireEscape = new Models.FireEscape()
                {
                    FireEscapeType = FireEscapePropertiesSettings.FireEscapeTypes![0],
                    FireEscapeMountType = FireEscapePropertiesSettings.FireEscapeMountTypes![0]
                },
                Created = DateTime.Now
            };
        }
    }
}
