using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyWebSocketProtocol.Client
{
    public class MyWebSocketClient
    {
        private readonly ClientWebSocket webSocket = new ClientWebSocket();

        private readonly Dictionary<string, (Type, Action<object>)> handlers =
            new Dictionary<string, (Type, Action<object>)>();

        public WebSocketState State => webSocket.State;

        public async Task ConnectAsync(string address, CancellationToken? cancellationToken = null)
        {
            await webSocket.ConnectAsync(
                new Uri(address),
                cancellationToken ?? CancellationToken.None);

            if (webSocket.State == WebSocketState.Open)
            {
#pragma warning disable CS4014
                Task.Run(Listen);
#pragma warning restore CS4014
            }
        }

        public Task StopAsync()
        {
            return webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closed by client",
                CancellationToken.None);
        }

        public Task SendAsync<T>(string command, T message)
        {
            var webSocketMessage = new MyWebSocketMessage
            {
                Command = command,
                Body = JsonSerializer.Serialize(message)
            };

            var buffer = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(webSocketMessage));

            return webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task Listen()
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                var receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Text && receiveResult.EndOfMessage)
                {
                    try
                    {
                        Handle(buffer.Array[0..(receiveResult.Count)]);
                    }
                    catch
                    {
                        //ignored
                    }
                }
            }
        }

        private void Handle(byte[] buffer)
        {
            var message = JsonSerializer.Deserialize<MyWebSocketMessage>(buffer);

            if (message == null)
                return;

            var messageHandlers = handlers.Where(x => x.Key == message.Command).ToList();

            if (messageHandlers.Count == 0)
                return;

            foreach (var handler in messageHandlers)
            {
                handler.Value.Item2(JsonSerializer.Deserialize(message.Body, handler.Value.Item1));
            }
        }

        public void On<T>(string name, Action<T> handler) where T : class
        {
            handlers.Add(name, (typeof(T), new Action<object>(param => handler((T)param))));
        }
    }
}
