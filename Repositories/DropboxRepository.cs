﻿using Dropbox.Api.Files;
using Dropbox.Api;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace FireEscape.Repositories
{
    public class DropboxRepository
    {
        readonly DropboxSettings dropboxSettings;
        readonly HttpClient httpClient = new HttpClient(new AndroidMessageHandler());

        public DropboxRepository(IOptions<DropboxSettings> dropboxSettings)
        {
            this.dropboxSettings = dropboxSettings.Value;
        }

        public async Task<string> UploadJsonAsync(string key, string value, string folder = "")
        {
            using var dbx = new DropboxClient(await GetTokenAsync());
            using var mem = new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
            var updated = await dbx.Files.UploadAsync(GetJsonPath(key, folder), WriteMode.Overwrite.Instance, body: mem);
            return updated.Id;
        }

        public async Task<string> DownloadJsonAsync(string key, string folder = "" )
        {
            using var dbx = new DropboxClient(await GetTokenAsync(), new DropboxClientConfig() { HttpClient = httpClient });
            using var response = await dbx.Files.DownloadAsync(GetJsonPath(key, folder));
            var s = await response.GetContentAsByteArrayAsync();
            return Encoding.Default.GetString(s);
        }

        public async Task<string> UploadAsync(string sourceFilePath, string destinationFilePath)
        {
            using var dbx = new DropboxClient(await GetTokenAsync());
            using var mem = new MemoryStream(await File.ReadAllBytesAsync(sourceFilePath));
            var updated = await dbx.Files.UploadAsync(GetAppPath() + destinationFilePath, WriteMode.Overwrite.Instance, body: mem);
            return updated.Id;
        }

        public async Task DownloadAsync(string sourceFilePath, string destinationFilePath)
        {
            using var dbx = new DropboxClient(await GetTokenAsync(), new DropboxClientConfig() { HttpClient = httpClient });
            using var response = await dbx.Files.DownloadAsync(GetAppPath() + sourceFilePath);
            var content = await response.GetContentAsByteArrayAsync();
            await File.WriteAllBytesAsync(destinationFilePath, content);
        }

        private string GetAppPath() => "/" + dropboxSettings.ApplicationFolderName + "/";

        private string GetJsonPath(string key, string folder) => GetAppPath()
            + (string.IsNullOrWhiteSpace(folder) ? string.Empty : folder + "/")
            + key
            + ".json";

        private async Task<string?> GetTokenAsync() // https://stackoverflow.com/questions/71524238/how-to-create-not-expires-token-in-dropbox-api-v2
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), dropboxSettings.TokenUri);
            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(dropboxSettings.AppKey + ":" + dropboxSettings.AppSecret));
            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
            request.Content = new StringContent("refresh_token="+ dropboxSettings.RefreshToken + "&grant_type=refresh_token");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var accessToken = await response.Content.ReadFromJsonAsync<AccessToken>();
                return accessToken.access_token;
            }

            return null;
        }

        /*
        public async Task<FullAccount> GetCurrentAccountAsync()
        {
            var token = await GetTokenAsync();
            using var dbx = new DropboxClient(token);
            return await dbx.Users.GetCurrentAccountAsync();
        }

        public async Task<ListFolderResult> ListFolderAsync(string folder)
        {
            using var dbx = new DropboxClient(await GetTokenAsync());
            return await dbx.Files.ListFolderAsync(GetAppPath() + folder + "/");
        }
        */
    }
    struct AccessToken
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
}