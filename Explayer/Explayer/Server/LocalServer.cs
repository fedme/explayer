using Explayer.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace Explayer.Server
{

    interface ILocalServer
    {
        Task<string> Start();
    }

    class LocalServer : ILocalServer
    {
        private readonly IHandleStaticFilesService _staticFilesService;

        public LocalServer(IHandleStaticFilesService staticFilesService)
        {
            _staticFilesService = staticFilesService;
        }

        public static string Url => "http://" + GetLocalIpAddress() + ":8787";

        public async Task<string> Start()
        {
            await _staticFilesService.InitializeStaticFiles();
            var filePath = _staticFilesService.DirectoryPath;

            // Start the web server
            await Task.Factory.StartNew(async () =>
            {
                using (var server = new WebServer(Url))
                {
                    // Register static files service
                    server.RegisterModule(new LocalSessionModule());
                    server.RegisterModule(new StaticFilesModule(filePath));
                    server.Module<StaticFilesModule>().UseRamCache = false;
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

            return Url;
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
    }
}
