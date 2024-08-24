using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace FireEscape.Repositories;

public class DropboxRepository(IOptions<FileHostingSettings> fileHostingSettings) : IFileHostingRepository
{
    readonly FileHostingSettings fileHostingSettings = fileHostingSettings.Value;
    readonly HttpClient httpClient = new HttpClient(new AndroidMessageHandler());

    public async Task<string> UploadJsonAsync(string key, string value, string folder = "")
    {
        using var dbx = await GetDropboxClient();
        using var mem = new MemoryStream(Encoding.UTF8.GetBytes(value ?? string.Empty));
        var updated = await dbx.Files.UploadAsync(GetJsonPath(key, folder), WriteMode.Overwrite.Instance, body: mem);
        return updated.Id;
    }

    public async Task<string> DownloadJsonAsync(string key, string folder = "")
    {
        using var dbx = await GetDropboxClient();
        try
        {
            using var response = await dbx.Files.DownloadAsync(GetJsonPath(key, folder));
            var s = await response.GetContentAsByteArrayAsync();
            return Encoding.Default.GetString(s);
        }
        catch (ApiException<DownloadError> ex)
        {
            var errorResponse = ex.ErrorResponse as DownloadError.Path;
            if (errorResponse != null && errorResponse.Value.IsNotFound)
                return string.Empty;
            throw;
        }
    }

    public async IAsyncEnumerable<string> DownloadJsonAsync(IAsyncEnumerable<string> keys, string folder = "")
    {
        using var dbx = await GetDropboxClient();
        await foreach (var key in keys)
        {
            using var response = await dbx.Files.DownloadAsync(GetJsonPath(key, folder));
            var s = await response.GetContentAsByteArrayAsync();
            yield return Encoding.Default.GetString(s);
        }
    }

    public async Task DeleteJsonAsync(string key, string folder = "")
    {
        using var dbx = await GetDropboxClient();
        await dbx.Files.DeleteV2Async(GetJsonPath(key, folder));
    }

    public async Task<string> UploadAsync(string sourceFilePath, string destinationFilePath)
    {
        using var dbx = await GetDropboxClient();
        using var mem = new MemoryStream(await File.ReadAllBytesAsync(sourceFilePath));
        var updated = await dbx.Files.UploadAsync(GetAppPath() + destinationFilePath, WriteMode.Overwrite.Instance, body: mem);
        return updated.Id;
    }

    public async Task DownloadAsync(string sourceFilePath, string destinationFilePath)
    {
        using var dbx = await GetDropboxClient();
        using var response = await dbx.Files.DownloadAsync(GetAppPath() + sourceFilePath);
        var content = await response.GetContentAsByteArrayAsync();
        await File.WriteAllBytesAsync(destinationFilePath, content);
    }

    public async IAsyncEnumerable<string> ListFolderAsync(string folder)
    {
        using var dbx = await GetDropboxClient();
        var result = await dbx.Files.ListFolderAsync(GetAppPath() + folder + "/");
        foreach (var file in result.Entries)
        {
            yield return file.Name;
        }
    }

    string GetAppPath() => "/" + fileHostingSettings.ApplicationFolderName + "/";

    string GetJsonPath(string key, string folder) => GetAppPath()
        + (string.IsNullOrWhiteSpace(folder) ? string.Empty : folder + "/")
        + key
        + ".json";

    async Task<string?> GetTokenAsync() // https://stackoverflow.com/questions/71524238/how-to-create-not-expires-token-in-dropbox-api-v2
    {
        using var request = new HttpRequestMessage(new HttpMethod("POST"), fileHostingSettings.TokenUri);
        var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(fileHostingSettings.AppKey + ":" + fileHostingSettings.AppSecret));
        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
        request.Content = new StringContent("refresh_token=" + fileHostingSettings.RefreshToken + "&grant_type=refresh_token");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();
            return accessToken.access_token;
        }
        return null;
    }

    async Task<DropboxClient> GetDropboxClient() => new DropboxClient(await GetTokenAsync(), new DropboxClientConfig() { HttpClient = httpClient });

    /*
    public async Task<FullAccount> GetCurrentAccountAsync()
    {
        var token = await GetTokenAsync();
        using var dbx = await GetDropboxClient();
        return await dbx.Users.GetCurrentAccountAsync();
    }

    */
}
record struct AccessToken
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}

class AndroidMessageHandler : HttpClientHandler // (HTTP 400 Bad Request On Download) https://github.com/dropbox/dropbox-sdk-dotnet/issues/77#issuecomment-1153300385
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri!.AbsolutePath.Contains("files/download"))
        {
            request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        }
        return base.SendAsync(request, cancellationToken);
    }
}
