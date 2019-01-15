using Acr.UserDialogs;
using Explayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

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
            var appsFolder = new DirectoryInfo(_staticFilesService.DirectoryPath);
            return appsFolder.GetDirectories().Select(folder => 
                new WebApp(folder.Name) {InstalledVersions = GetAppInstalledVersionStrings(folder.Name)}).ToList();
        }

        /// <summary>
        /// Downloads an app from the server
        /// </summary>
        /// <param name="serverUrl">Url of the app server</param>
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
                    foreach (var file in di.GetFiles())
                        file.Delete();
                    foreach (var dir in di.GetDirectories())
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
            catch (Exception e) // TODO: Better exception handling
            {
                // Show error toast
                var toastConfig = new ToastConfig($"Error while downloading app: {e.Message}")
                    .SetDuration(4000);
                UserDialogs.Instance.Toast(toastConfig);
            }
        }

        private List<string> GetAppInstalledVersionStrings(string appName)
        {
            var appFolder = new DirectoryInfo(Path.Combine(_staticFilesService.DirectoryPath, appName));
            return appFolder.GetDirectories().Select(folder => folder.Name).ToList();
        }

        private static bool RemoteFileExists(string url)
        {
            try
            {
                if (WebRequest.Create(url) is HttpWebRequest request)
                {
                    request.Method = "HEAD";
                    if (request.GetResponse() is HttpWebResponse response)
                    {
                        response.Close();
                        return (response.StatusCode == HttpStatusCode.OK);
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }   
    }
}
