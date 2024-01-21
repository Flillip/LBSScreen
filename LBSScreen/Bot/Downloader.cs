using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LBSScreen.Bot
{
    internal class Downloader
    {
        internal readonly string _downloadPath;

        public Downloader()
        {
            _downloadPath = Settings.GetData<string>("downloadPath");
        }

        public async Task Download(string url, string name, string extension)
        {
            string localFilePath = Path.Combine(_downloadPath, name + extension);
            if (File.Exists(localFilePath) || url == "" || name == "" || extension == "") return;

            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                byte[] fileContent = await response.Content.ReadAsByteArrayAsync();

                File.WriteAllBytes(localFilePath, fileContent);
                Logger.Log($"File downloaded successfully to: {localFilePath}");
            }

            else
                Logger.Error($"Error downloading file. Status code: {response.StatusCode}");
        }
    }
}
