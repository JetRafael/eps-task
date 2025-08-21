using EPS.Data.Services;
using EPS.Server.Models;
using EPS.Server.Utilities;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;

namespace EPS.Server.Services
{
    public class CodeGeneratorService: WebSocketService
    {
        private readonly DataService _dataService;
        public CodeGeneratorService(DataService dataService) {
            _dataService = dataService;
        }
        public async Task HandleRequest(HttpContext context, WebSocket webSocket) {
            string connectionId = Guid.NewGuid().ToString();
            ConcurrentDictionary<string, WebSocket> connections = await this.HandleConnectionAsync(connectionId, context, webSocket);

            try
            {
                await ReceiveMessages(connectionId, webSocket);
            }
            finally
            {
                connections.TryRemove(connectionId, out _);
            }

        }
        private async Task ReceiveMessages(string connectionId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        CodeGeneratorRequest parsedRequestData = null;

                        if (!String.IsNullOrEmpty(message)) {
                           /**
                            * This could potentially throw exception
                            * Improvement: Do better exception handling
                            * */
                            parsedRequestData = JsonConvert.DeserializeObject<CodeGeneratorRequest>(message);
                        }
                        
                        Console.WriteLine($"Connection {connectionId} received: {message}");

                        if (parsedRequestData != null)
                        {
                            // Process the message
                            await ProcessMessage(connectionId, parsedRequestData, webSocket);
                        }
                        else {
                            //
                            // Improvement: Do better exception and validation error handling
                            //
                            await SendMessageAsync(webSocket, "Invalid Data");
                        }
                        
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Connection closed by client",
                            CancellationToken.None);
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}");
                    break;
                }
            }
        }

        private async Task ProcessMessage(string connectionId, CodeGeneratorRequest requestData, WebSocket webSocket)
        {
            //
            // Do requestion data validation
            //
            if (requestData.Count > 2000) {
                await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeGeneratorResponse() { Result = false }));
                return;
            }

            if (requestData.Length < 7 || requestData.Length > 8) {
                await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeGeneratorResponse() { Result = false }));
                return;
            }

            string[] codes = RandomCodeGenerator.GetRandomCodes(requestData.Count, requestData.Length);

            //
            // Save the newly generated codes into DB
            //
            if (codes.Length > 0) {
                _dataService.SaveDiscountCodes(codes);
            }
         
            await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeGeneratorResponse() { Result = true }));
        }
    }
}
