using FireEscape.Models;
using System.Text.Json;

namespace FireEscape.Services
{
    public class ProtocolService
    {

        List<Protocol> protocolList = new();
        public async Task<List<Protocol>> GetProtocols()
        {
            if (protocolList?.Count > 0)
                return protocolList;
            /*
            using var stream = await FileSystem.OpenAppPackageFileAsync("protocoldata.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            _protocolList = JsonSerializer.Deserialize(contents, ProtocolContext.Default.ListProtocol)  ;
            */

            protocolList.Add(new Protocol() { Name = "test name1", Details = "test details1", Location = "test location1", Image = "dotnet_bot.png" });
            protocolList.Add(new Protocol() { Name = "test name2", Details = "test details2", Location = "test location2", Image = "dotnet_bot.png" });
            return protocolList;
        }

    }
}
