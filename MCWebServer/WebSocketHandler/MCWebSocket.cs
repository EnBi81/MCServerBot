using MCWebServer.Log;
using MCWebServer.MinecraftServer;
using MCWebServer.MinecraftServer.Enums;
using MCWebServer.PermissionControll;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCWebServer.WebSocketHandler
{
    /// <summary>
    /// Handles a single websocket instance.
    /// </summary>
    public class MCWebSocket
    {
        private readonly WebSocket _socket;
        private bool _readInput = true; // true if it should read input from the socket, false if not.
        private readonly MCSocketInputHandler _inputHandler;

        /// <summary>
        /// If the socket is actively trying reading input.
        /// </summary>
        public bool IsOpen { get => _readInput; }

        /// <summary>
        /// Discord user associated with this socket.
        /// </summary>
        public DiscordUser DiscordUser { get; }

        /// <summary>
        /// Code of the Discord user.
        /// </summary>
        public string Code { get; }

        public MCWebSocket(WebSocket socket, DiscordUser discordUser, string code)
        {
            _inputHandler = new MCSocketInputHandler(this);
            _socket = socket;
            DiscordUser = discordUser;
            Code = code;
        }

        /// <summary>
        /// Starts listening to the socket and sends startup data.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var receive = ReceiveDataAsync(MessageReceived);
            await SendStartupData();
            await receive;
        }

        /// <summary>
        /// Parses the request, and sends it to the input handler for further inspection.
        /// </summary>
        /// <param name="message">whole request text</param>
        /// <returns></returns>
        private async Task MessageReceived(string message)
        {
            string? requestName;
            JObject? requestData;
            try
            {
                var data = JsonConvert.DeserializeObject<WebSocketData>(message);
                requestName = data?.Request;
                requestData = data?.Data;
            } catch
            {
                string errorMessage = $"Invalid request from {DiscordUser.Username}: " + message;
                LogService.GetService<WebLogger>().Log("socket", errorMessage);
                string error = MessageFormatter.ErrorMessage(errorMessage);
                await SendMessage(error);
                return;
            }
            
            await _inputHandler.HandleInput(requestName, requestData);



            //if(requestName == "toggle")
            //{
            //    LogService.GetService<WebLogger>().Log("socket", "Toggle request from " + DiscordUser.Username);
            //    try
            //    {
            //        var status = ServerPark.Keklepcso.Status;
            //        if (status == ServerStatus.Starting || status == ServerStatus.ShuttingDown)
            //            throw new Exception("Server is Loading. Please wait.");

            //        if (ServerPark.Keklepcso.IsRunning)
            //            ServerPark.Keklepcso.Shutdown(DiscordUser.Username);
            //        else
            //            ServerPark.Keklepcso.Start(DiscordUser.Username);
            //    }
            //    catch (Exception ex)
            //    {
            //        var command = MessageFormatter.Log(requestData, (int)LogMessage.LogMessageType.User_Message);
            //        var mess = MessageFormatter.Log(ex.Message, (int)LogMessage.LogMessageType.Error_Message);
            //        await SendMessage(command);
            //        await SendMessage(mess);
            //    }
                
            //}
            
            //else if(requestName == "logout")
            //{
            //    LogService.GetService<WebLogger>().Log("socket", "Logout request from " + DiscordUser.Username);
            //    await Close();
            //}

            //else if(requestName == "addCommand")
            //{
            //    LogService.GetService<WebLogger>().Log("socket", $"Command received from {DiscordUser.Username}, command: {requestData}");
            //    try
            //    {
            //        ServerPark.Keklepcso.WriteCommand(requestData, DiscordUser.Username);
            //    }
            //    catch(Exception ex)
            //    {
            //        var command = MessageFormatter.Log(requestData, (int)LogMessage.LogMessageType.User_Message);
            //        var mess = MessageFormatter.Log(ex.Message, (int)LogMessage.LogMessageType.Error_Message);
            //        await SendMessage(command);
            //        await SendMessage(mess);
            //    }
                
            //}
        }


        private async Task SendStartupData()
        {
            //var mcServer = ServerPark.Keklepcso;
            //var status = MessageFormatter.StatusUpdate(mcServer.Status, mcServer.OnlineFrom, mcServer.StorageSpace);

            //await SendMessage(status);

            //foreach (var activePlayer in mcServer.OnlinePlayers)
            //{
            //    var playerData = MessageFormatter.PlayerJoin(activePlayer.Username, activePlayer.OnlineFrom.Value, activePlayer.PastOnline);
            //    await SendMessage(playerData);
            //}

            //var logs = MessageFormatter.Log(mcServer.Logs.TakeLast(50));
            //await SendMessage(logs);
            await Task.Delay(1);
        }



        #region Low level network stuff, no business logic

        /// <summary>
        /// Receiv
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveDataAsync(Func<string, Task> MessageHandler)
        {
            try
            {
                var buffer = new byte[1024];
                WebSocketReceiveResult result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                StringBuilder resultBuilder = new StringBuilder();

                while (_readInput && !result.CloseStatus.HasValue)
                {
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await Close();
                        return;
                    }

                    if(result.MessageType != WebSocketMessageType.Text)
                    {
                        continue;
                    }

                    resultBuilder.Append(Encoding.UTF8.GetString(buffer));
                    Array.Clear(buffer, 0, buffer.Length);

                    if (result.EndOfMessage)
                    {
                        string res = resultBuilder.ToString();
                        try
                        {
                            await MessageHandler(res);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                        

                        resultBuilder.Clear();
                    }



                    result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
            }
            catch { }
            await Close();
        }

        /// <summary>
        /// Sends a text message through the socket.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SendMessage(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Closes the websocket nicely.
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            LogService.GetService<WebLogger>().Log("socket", "Closing socket for " + DiscordUser.Username);

            _readInput = false;
            try
            {
                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch { }
        }

        /// <summary>
        /// Just to deserialize the incoming data.
        /// </summary>
        private class WebSocketData
        {
            public string? Request { get; set; }
            public JObject? Data { get; set; }
        }

        #endregion
    }
}
