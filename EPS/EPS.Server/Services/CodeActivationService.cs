using EPS.Data.DataModel;
using EPS.Data.Services;
using EPS.Server.Models;
using EPS.Server.Utilities;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;

namespace EPS.Server.Services
{
    public class CodeActivationService : WebSocketService
    {
        private readonly DataService _dataService;

        public CodeActivationService(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task HandleRequest(HttpContext context, WebSocket webSocket)
        {
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
                        CodeActivationRequest parsedRequestData = null;

                        if (!String.IsNullOrEmpty(message))
                        {
                            /**
                             * This could potentially throw exception
                             * Improvement: Do better exception handling
                             * */
                            parsedRequestData = JsonConvert.DeserializeObject<CodeActivationRequest>(message);
                        }

                        Console.WriteLine($"Connection {connectionId} received: {message}");

                        if (parsedRequestData != null)
                        {
                            // Process the message
                            await ProcessMessage(connectionId, parsedRequestData, webSocket);
                        }
                        else
                        {
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

        private async Task ProcessMessage(string connectionId, CodeActivationRequest requestData, WebSocket webSocket)
        {
            //
            // Validate if code is empty
            //
            if (String.IsNullOrEmpty(requestData.Code)) {
                await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeActivationReponse() { Result = 400 }));
                return;
            }

            //
            // Validate if code is valid/invalid
            //
            DiscountCode activationCode = _dataService.GetDiscountCode(requestData.Code);
            if (activationCode != null)
            {
                if (activationCode.IsActivated == true) {
                    await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeActivationReponse() { Result = 400 }));
                    return;
                }
            }
            else {
                await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeActivationReponse() { Result = 400 }));
                return;
            }

            //Do activate the discount code
            _dataService.ActivateDiscountCode(activationCode);
            await SendMessageAsync(webSocket, JsonConvert.SerializeObject(new CodeActivationReponse() { Result = 201 }));
        }
    }
}
