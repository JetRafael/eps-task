
using EPS.Server.Interfaces;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace EPS.Server.Services
{
    public abstract class WebSocketService: IWebSocket
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections = new();

        public Task<ConcurrentDictionary<string, WebSocket>> HandleConnectionAsync(string connectionId, HttpContext context, WebSocket webSocket)
        {
            
            _connections.TryAdd(connectionId, webSocket);

            return Task.FromResult(_connections);
        }


        //
        // Check if we still need the SendMessageAsync
        //
        public async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
    }
}
