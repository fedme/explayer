using Explayer.Services;
using System;
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
            await StartLocalServer();
        }

        private async Task StartLocalServer()
        {
            var url = "http://" + GetLocalIpAddress() + ":8787";
            var htmlService = DependencyService.Get<IHandleStaticFilesService>();
            await htmlService.InitializeStaticFiles();
            var filePath = htmlService.DirectoryPath;


            // Handle when your app starts
            await Task.Factory.StartNew(async () =>
            {
                using (var server = new WebServer(url))
                {
                    server.RegisterModule(new LocalSessionModule());
                    server.RegisterModule(new StaticFilesModule(filePath));
                    server.Module<StaticFilesModule>().UseRamCache = true;
                    server.Module<StaticFilesModule>().DefaultExtension = ".html";
                    server.Module<StaticFilesModule>().DefaultDocument = "index.html";
                    server.Module<StaticFilesModule>().UseGzip = false;
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
