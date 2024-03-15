using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Services
{
    public class ProtocolService
    {
        List<Protocol> protocolList = new();
        NewProtocolSettings settings;

        public ProtocolService(IOptions<NewProtocolSettings> settings) 
        {
            this.settings = settings.Value;
        

            //this.configuration = configuration;
            /*
            // Bind a configuration section to an instance of Settings class
            var settings = configuration.GetSection("ProtocolSettings").Get<DefaultProtocolSettings>();

            foreach (var endpoint in settings.Locatons)
            {
                Console.WriteLine(endpoint);
            }


            // Read simple values
            Console.WriteLine($"Server: {settings.Server}");
            Console.WriteLine($"Database: {settings.Database}");

            // Read nested objects
            Console.WriteLine("Endpoints: ");

            foreach (Endpoint endpoint in settings.Endpoints)
            {
                Console.WriteLine($"{endpoint.IPAddress}:{endpoint.Port}");
            }
            */
        }    

        public async Task<Protocol> CreateProtocol() 
        {
            var protocol = new Protocol() {
                Name = settings.Name,
                Image = AppResources.NoPhoto!,
                ProtocolNum = settings.ProtocolNum,
                Location = settings.Location,
                ProtocolDate = DateTime.Today,
                Address = string.Empty,
                FireEscapeNum = settings.FireEscapeNum,
                Details = string.Empty,
                Created = DateTime.Now
            };

            await SaveProtocolAsync(protocol);
            return protocol;
        } 
 
        public async Task SaveProtocolAsync(Protocol protocol)
        {
            var filePath = protocol.SourceFile?? Path.Combine(AppSettingsExtension.ContentFolder, Guid.NewGuid().ToString() + ".json");
            protocol.Updated = DateTime.Now;
            using var fs = File.Create(filePath);
            await JsonSerializer.SerializeAsync(fs, protocol);
            protocol.SourceFile = filePath;
            if (!protocolList.Contains(protocol))
                protocolList.Insert(0, protocol);
        }

        public void DeleteProtocol(Protocol protocol)
        {
            if (protocol.HasImage)
                File.Delete(protocol.Image);
            if (File.Exists(protocol.SourceFile))
                File.Delete(protocol.SourceFile);
            protocolList.Remove(protocol);
        }

        public async Task CreatePdfAsync(Protocol protocol)
        {
            await PdfHelper.MakePdfFileAsync(protocol);
        }

        public async Task<List<Protocol>> GetProtocolsAsync()
        {
            if (protocolList.Count > 0)
                return protocolList;
            var files = Directory.GetFiles(AppSettingsExtension.ContentFolder, "*.json");
            foreach (var file in files)
            {
                using (var fs = File.OpenRead(file))
                {
                    Protocol? protocol = null;
                    try
                    {
                        protocol = await JsonSerializer.DeserializeAsync<Protocol>(fs);
                    
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error: {ex.Message}");
                        protocol = CreateBrokenProtocol(file);
                    }

                    if (protocol != null)
                    {
                        protocol.SourceFile = file;
                        protocolList.Add(protocol);
                    }
                }
            }

            protocolList = protocolList.OrderByDescending(item => item.Created).ToList();
            return protocolList;
        }


        public async Task AddProtocolPhotoAsync(Protocol protocol)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    var photoFilePath = Path.Combine(AppSettingsExtension.ContentFolder, photo.FileName);
                    using (var photoStream = await photo.OpenReadAsync())
                    using (var outputFile = File.Create(photoFilePath))
                    {
                        await photoStream.CopyToAsync(outputFile);
                    }
                    if (protocol.HasImage)
                        File.Delete(protocol.Image);

                    protocol.Image = photoFilePath;
                }
            }
        }

        private Protocol CreateBrokenProtocol(string file)
        {
            var protocol = new Protocol()
            {
                Name = AppResources.BrokenData,
                Image = AppResources.NoPhoto!,
                ProtocolNum = 0,
                Location = string.Empty,
                Address = string.Empty,
                FireEscapeNum = 0,
                Details = string.Empty,
                SourceFile= file
            };
            return protocol;
        }
    }
}
