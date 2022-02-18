using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MyWebSocketProtocol
{
    public sealed class MyConnectionManager
    {
        private readonly ILogger<MyConnectionManager> logger;

        private readonly ConcurrentDictionary<string, WebSocket> connections =
            new ConcurrentDictionary<string, WebSocket>();

        public MyConnectionManager(ILogger<MyConnectionManager> logger)
        {
            this.logger = logger;
        }

        public string AddSocket(WebSocket socket)
        {
            var connectiodId = GetNewConnectionId();

            connections.TryAdd(connectiodId, socket);

            return connectiodId;
        }

        public Task RemoveSocket(string id)
        {
            connections.TryRemove(id, out var socket);

            return socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                    "Closed by server",
                                    CancellationToken.None);
        }

        private async Task SendMessageToOthers(MyWebSocketMessage message, string sender)
        {
            var otherClients = connections.Where(x => x.Key != sender).Select(x => x.Value);

            message.Command = "Send";
            var arrayBuffer = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(message));

            foreach (var client in otherClients)
            {
                try
                {
                    await client.SendAsync(buffer: arrayBuffer,
                        messageType: WebSocketMessageType.Text,
                        endOfMessage: true,
                        cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cannot send message");
                }
            }
        }

        private string GetNewConnectionId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task ProcessMessage(byte[] messageBytes, string sender)
        {
            try
            {
                var str = Encoding.UTF8.GetString(messageBytes);

                var message = JsonSerializer.Deserialize<MyWebSocketMessage>(messageBytes);

                if (message != null && message.Command == "SendToOthers")
                    await SendMessageToOthers(message, sender);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot process message");
            }
        }
    }
}