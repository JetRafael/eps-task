using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace EPS.Server.Interfaces
{
    public interface IWebSocket
    {
        Task<ConcurrentDictionary<string, WebSocket>> HandleConnectionAsync(string connectinoId, HttpContext context, WebSocket webSocket);
        Task SendMessageAsync(WebSocket webSocket, string message);
    }
}
