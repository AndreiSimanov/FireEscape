using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
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
            var filePath = protocol.File?? Path.Combine(AppSettingsExtension.ContentFolder, Guid.NewGuid().ToString() + ".json");
            protocol.Updated = DateTime.Now;
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
              await JsonSerializer.SerializeAsync(fs, protocol);
            protocol.File = filePath;
            if (!protocolList.Contains(protocol))
                protocolList.Insert(0, protocol);
        }

        public void DeleteProtocol(Protocol protocol)
        {
            DeleteImageFile(protocol.Image);
            if (File.Exists(protocol.File))
                File.Delete(protocol.File);
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
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    Protocol? protocol = null;
                    try
                    {
                        protocol = await JsonSerializer.DeserializeAsync<Protocol>(fs);
                    
                    }
                    catch (Exception)
                    {
                        protocol = CreateBrokenProtocol(file);
                    }

                    if (protocol != null)
                    {
                        protocol.File = file;
                        protocolList.Add(protocol);
                    }
                }
            }

            /*
            protocolList.Add(new Protocol() { Name = "Протокол 1", Details = "Test details1", Location = "Test location1"});
            protocolList.Add(new Protocol() { Name = "Протокол 2", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 3", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 4", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 5", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 6", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 7", Details = "Test details2", Location = "Test location2"});
            protocolList.Add(new Protocol() { Name = "Протокол 8", Details = "Test details2", Location = "Test location2"});
            */
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
                    using (var outputFile = new FileStream(photoFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await photoStream.CopyToAsync(outputFile);
                    }
                    DeleteImageFile(protocol.Image);
                    protocol.Image = photoFilePath;
                }
            }
        }
        private void DeleteImageFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            if (!string.Equals(path, AppResources.NoPhoto) &&
                File.Exists(path))
                File.Delete(path);
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
                File= file
            };
            return protocol;
        }
    }
}
