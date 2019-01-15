using Acr.UserDialogs;
using Explayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Explayer.Services
{

    public interface IAppManagerService
    {
        Task DownloadApp(string serverUrl, string appName, string appVersion);
        List<WebApp> GetInstalledApps();
    }

    public class AppManagerService : IAppManagerService
    {

        private readonly IHandleStaticFilesService _staticFilesService;

        public AppManagerService(
            IHandleStaticFilesService staticFilesService
            )
        {
            _staticFilesService = staticFilesService;
        }

        /// <summary>
        /// Get the list of installed Web Apps
        /// </summary>
        /// <returns>List of installed Web Apps</returns>
        public List<WebApp> GetInstalledApps()
        {
            var apps = new List<WebApp>();
            var appsFolder = new DirectoryInfo(_staticFilesService.DirectoryPath);
            foreach (var folder in appsFolder.GetDirectories())
            {
                var app = new WebApp(folder.Name);
                app.InstalledVersions = GetAppInstalledVersionStrings(folder.Name);
                apps.Add(app);
            }
            return apps;
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
            var zipUrl = $"{serverUrl}/apps/{appName}/{zipName}";

            try
            {
                // Check if app zip file exists on server
                if (!RemoteFileExists(zipUrl))
                    throw new FileNotFoundException("App archive does not exist on the server");

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

                // Show success toast
                var toastConfig = new ToastConfig($"App Downloaded!").SetDuration(4000);
                UserDialogs.Instance.Toast(toastConfig);
            }
            catch (Exception e) //TODO: Better exception handling
            {
                // Show error toast
                var toastConfig = new ToastConfig($"Error while downloading app: {e.Message}").SetDuration(4000);
                UserDialogs.Instance.Toast(toastConfig);
            }
        }

        private List<string> GetAppInstalledVersionStrings(string appName)
        {
            var versionStrings = new List<string>();
            var appFolder = new DirectoryInfo(Path.Combine(_staticFilesService.DirectoryPath, appName));
            foreach (var folder in appFolder.GetDirectories())
            {
                versionStrings.Add(folder.Name);
            }

            return versionStrings;
        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
    }
}
