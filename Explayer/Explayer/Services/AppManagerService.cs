using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Explayer.Services
{

    public interface IAppManagerService
    {
        Task DownloadApp(string serverUrl, string appName, string appVersion);
    }

    public class AppManagerService : IAppManagerService
    {

        private readonly IHandleStaticFilesService _staticFilesService;

        public AppManagerService(IHandleStaticFilesService staticFilesService)
        {
            _staticFilesService = staticFilesService;
        }

        /// <summary>
        /// Downloads an app from the server
        /// </summary>
        /// <param name="appName">Name of the app</param>
        /// <param name="appVersion">Version string</param>
        /// <returns></returns>
        public async Task DownloadApp(string serverUrl, string appName, string appVersion)
        {
            var zipName = $"{appName}-v{appVersion}.zip";
            var zipUrl = $"{serverUrl}/{zipName}";

            // Create app directory if it does not exist
            var appFolder = Path.Combine(_staticFilesService.DirectoryPath, appName);
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);

            // Create app version directory if it does not exist
            var appVersionFolder = Path.Combine(appFolder, appVersion);
            if (!Directory.Exists(appVersionFolder)) Directory.CreateDirectory(appVersionFolder);
            else {
                // if directory already exists, delete everything in it
                var di = new DirectoryInfo(appVersionFolder);
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }

            // Download app zip file
            var zipPath = Path.Combine(appVersionFolder, zipName);
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(zipUrl, zipPath);
            }

            // Extract app zip file
            ZipFile.ExtractToDirectory(zipPath, appVersionFolder);
            File.Delete(zipPath);
        }
    }
}
