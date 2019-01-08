using Explayer.Server;
using Explayer.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Explayer
{
    public partial class App : Application
    {

        private readonly WebView _webView = new WebView();
        private IHandleStaticFilesService StaticFilesService;

        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage
            {
                Content = _webView,
            };
        }

        protected override async void OnStart()
        {

            // Initialize static files and get their path
            StaticFilesService = DependencyService.Get<IHandleStaticFilesService>();

            // Setup local web server
            await StartLocalServer();

            // Download test stimuli and extract it
            var appName = "stimuli";
            var appVersion = "1.0.0";
            await DownloadApp(appName, appVersion);
        }

        private async Task DownloadApp(string appName, string appVersion)
        {
            var zipName = appName + "-v" + appVersion + ".zip";
            var zipUrl = "https://static.isearchlab.org/explayer/apps/" + zipName;

            // Create app directory if it does not exist
            var appFolder = Path.Combine(StaticFilesService.DirectoryPath, appName);
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);

            // Create app version directory if it does not exist
            var appVersionFolder = Path.Combine(appFolder, appVersion);
            if (!Directory.Exists(appVersionFolder)) Directory.CreateDirectory(appVersionFolder);

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

        private async Task StartLocalServer()
        {
            // Generate server URL
            var url = "http://" + GetLocalIpAddress() + ":8787";

            
            await StaticFilesService.InitializeStaticFiles();
            var filePath = StaticFilesService.DirectoryPath;

            // Start the web server
            await Task.Factory.StartNew(async () =>
            {
                using (var server = new WebServer(url))
                {
                    // Register static files service
                    server.RegisterModule(new LocalSessionModule());
                    server.RegisterModule(new StaticFilesModule(filePath));
                    server.Module<StaticFilesModule>().UseRamCache = true;
                    server.Module<StaticFilesModule>().DefaultExtension = ".html";
                    server.Module<StaticFilesModule>().DefaultDocument = "index.html";
                    server.Module<StaticFilesModule>().UseGzip = false;

                    // Register socket service
                    server.RegisterModule(new WebSocketsModule());
                    server.Module<WebSocketsModule>().RegisterWebSocketsServer<SocketServer>("/socket");

                    // Run server
                    await server.RunAsync();
                }
            });

            _webView.Source = url;
        }

        private static string GetLocalIpAddress()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Address.ToString();
            }
            finally
            {
                listener.Stop();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
