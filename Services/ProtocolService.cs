using FireEscape.Resources.Languages;
using System.Text.Json;

namespace FireEscape.Services
{
    public class ProtocolService
    {
        List<Protocol> protocolList = new();
 
        public async Task SaveProtocolAsync(Protocol protocol)
        {
            var filePath = protocol.File?? Path.Combine(AppRes.ContentFolder, Guid.NewGuid().ToString() + ".json");
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
            var files = Directory.GetFiles(AppRes.ContentFolder, "*.json");
            foreach (var file in files)
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    var protocol = await JsonSerializer.DeserializeAsync<Protocol>(fs);
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
                    var photoFilePath = Path.Combine(AppRes.ContentFolder, photo.FileName);
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
    }
}
