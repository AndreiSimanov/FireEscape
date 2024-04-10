using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Repositories
{
    public class LocalFileRepository : IProtocolRepository
    {
        readonly ApplicationSettings applicationSettings;
        readonly NewProtocolSettings newProtocolSettings;
        readonly StairsSettings stairsSettings;

        public LocalFileRepository(IOptions<ApplicationSettings> applicationSettings, 
            IOptions<NewProtocolSettings> newProtocolSettings,
            IOptions<StairsSettings> stairsSettings)
        {
            this.applicationSettings = applicationSettings.Value;
            this.newProtocolSettings = newProtocolSettings.Value;
            this.stairsSettings = stairsSettings.Value;
        }

        public async Task<Protocol> CreateProtocolAsync()
        {
            var protocol = CreateDefaultProtocol();
            await SaveProtocolAsync(protocol);
            return protocol;
        }

        public async Task<Protocol> CopyProtocolAsync(Protocol protocol)
        {
            var newProtocol = (Protocol)protocol.Clone();
            newProtocol.Id = null;
            newProtocol.Image = Protocol.NO_PHOTO;
            newProtocol.FireEscapeNum = newProtocol.FireEscapeNum + 1;
            newProtocol.Stairs = CreateDefaultStairs();
            newProtocol.Created = DateTime.Now;
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

        public async Task DeleteProtocol(Protocol protocol)
        {
            await Task.Run(() =>
            {
                if (protocol.HasImage)
                    File.Delete(protocol.Image!);
                if (File.Exists(protocol.Id))
                    File.Delete(protocol.Id);
            });
        }

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
                    protocol = new Protocol() { Id = file.FullName, Image = Protocol.NO_PHOTO, FireEscapeObject = AppResources.BrokenData };

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
                    protocol = new Protocol() { Id = file.FullName, Image = Protocol.NO_PHOTO, FireEscapeObject = AppResources.BrokenData };

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

        private Protocol CreateDefaultProtocol()
        {
            return new Protocol()
            {
                Image = Protocol.NO_PHOTO,
                ProtocolNum = newProtocolSettings.ProtocolNum,
                Location = newProtocolSettings.Location,
                ProtocolDate = DateTime.Today,
                FireEscapeNum = newProtocolSettings.FireEscapeNum,
                Stairs = CreateDefaultStairs(),
                Created = DateTime.Now
            };
        }

        private Stairs CreateDefaultStairs()
        {
            return new Stairs()
            {
                StairsHeight = new ServiceabilityProperty<float?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
                StairsWidth = new ServiceabilityProperty<int?>() { Serviceability = stairsSettings.ServiceabilityTypes![0] },
                StairsType = stairsSettings.StairsTypes![0],
                StairsMountType = stairsSettings.StairsMountTypes![0],
                StairsElements = new ObservableCollection<BaseStairsElement> {
                    new StepsP1(){ Order = 10}, 
                    new SupportВeamsP1(){ Order = 20},
                    new StepsP2(){ Order = 10},
                    new FenceP2(){ Order = 20},
                    new StairwayP2(){ Order = 30},
                    new PlatformP2(){ Order = 40}
                }
            };
        }
    }
}
