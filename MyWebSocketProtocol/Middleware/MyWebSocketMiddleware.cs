using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MyWebSocketProtocol.Middleware
{
    class MyWebSocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MyConnectionManager connectionManager;

        public MyWebSocketMiddleware(RequestDelegate next, MyConnectionManager connectionManager)
        {
            this.next = next;
            this.connectionManager = connectionManager;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest
                && context.Request.Path == "/messages")
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();

                var id = connectionManager.AddSocket(socket);

                await Loop(socket, id);

                await connectionManager.RemoveSocket(id);
            }
            else
            {
                await next(context);
            }
        }

        private async Task Loop(WebSocket socket, string connectionId)
        {
            while (socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.EndOfMessage && result.Count <= 1024)
                {
                    await connectionManager.ProcessMessage(buffer.Array[..result.Count], connectionId);
                }
            }
        }
    }
}
