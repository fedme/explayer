using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Swan;

namespace Explayer.Server
{
    /// <summary>
    /// Defines a very simple chat server
    /// </summary>
    public class SocketServer : WebSocketsServer
    {
        public SocketServer()
            : base(true)
        {
            // placeholder
        }


        /// <inheritdoc />
        protected override void OnMessageReceived(IWebSocketContext context, byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {
            //foreach (var ws in WebSockets.Where(ws => ws != context))
            foreach (var ws in WebSockets)
            {
                Send(ws, rxBuffer.ToText());

                // Show toast
                var toastConfig = new ToastConfig("New message: " + rxBuffer.ToText()).SetDuration(3000);
                UserDialogs.Instance.Toast(toastConfig);
            }
        }


        /// <inheritdoc />
        public override string ServerName => nameof(SocketServer);

        /// <inheritdoc />
        protected override void OnClientConnected(
            IWebSocketContext context,
            System.Net.IPEndPoint localEndPoint,
            System.Net.IPEndPoint remoteEndPoint)
        {
            Send(context, "Welcome to the chat room!");

            foreach (var ws in WebSockets.Where(ws => ws != context))
            {
                Send(ws, "Someone joined the chat room.");
            }
        }

        /// <inheritdoc />
        protected override void OnFrameReceived(IWebSocketContext context, byte[] rxBuffer,
            IWebSocketReceiveResult rxResult)
        {
            // placeholder
        }

        /// <inheritdoc />
        protected override void OnClientDisconnected(IWebSocketContext context)
        {
            Broadcast("Someone left the chat room.");
        }
    }
}
