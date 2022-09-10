using MCWebServer.Log;
using MCWebServer.MinecraftServer;
using MCWebServer.PermissionControll;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCWebServer.WebSocketHandler
{
    public class MCWebSocket
    {
        private readonly WebSocket _socket;
        private bool _readInput = true;

        public bool IsOpen { get => _readInput; }
        public DiscordUser DiscordUser { get; }
        public string Code { get; }

        public MCWebSocket(WebSocket socket, DiscordUser discordUser, string code)
        {
            _socket = socket;
            DiscordUser = discordUser;
            Code = code;
        }

        public async Task Initialize()
        {
            await SendStartupData();
            await ReceiveDataAsync();
        }

        private async Task MessageReceived(string message)
        {
            var data = JsonConvert.DeserializeObject<WebSocketData>(message);
            string requestName = data.Request;
            string requestData = data.Data;

            if(requestName == "toggle")
            {
                LogService.GetService<WebLogger>().Log("socket", "Toggle request from " + DiscordUser.Username);
                try
                {
                    var status = ServerPark.Keklepcso.Status;
                    if (status == ServerStatus.Starting || status == ServerStatus.ShuttingDown)
                        throw new Exception("Server is Loading. Please wait.");

                    if (ServerPark.Keklepcso.IsRunning())
                        ServerPark.Keklepcso.Shutdown(DiscordUser.Username);
                    else
                        ServerPark.Keklepcso.Start(DiscordUser.Username);
                }
                catch (Exception ex)
                {
                    var command = MessageFormatter.Log(requestData, (int)LogMessage.LogMessageType.User_Message);
                    var mess = MessageFormatter.Log(ex.Message, (int)LogMessage.LogMessageType.Error_Message);
                    await SendMessage(command);
                    await SendMessage(mess);
                }
                
            }
            
            else if(requestName == "logout")
            {
                LogService.GetService<WebLogger>().Log("socket", "Logout request from " + DiscordUser.Username);
                await Close();
            }

            else if(requestName == "addCommand")
            {
                LogService.GetService<WebLogger>().Log("socket", $"Command received from {DiscordUser.Username}, command: {requestData}");
                try
                {
                    ServerPark.Keklepcso.WriteCommand(requestData, DiscordUser.Username);
                }
                catch(Exception ex)
                {
                    var command = MessageFormatter.Log(requestData, (int)LogMessage.LogMessageType.User_Message);
                    var mess = MessageFormatter.Log(ex.Message, (int)LogMessage.LogMessageType.Error_Message);
                    await SendMessage(command);
                    await SendMessage(mess);
                }
                
            }
        }


        private async Task SendStartupData()
        {
            var mcServer = ServerPark.Keklepcso;
            var status = MessageFormatter.StatusUpdate(mcServer.Status, mcServer.OnlineFrom, mcServer.StorageSpace);

            await SendMessage(status);

            foreach (var activePlayer in mcServer.OnlinePlayers)
            {
                var playerData = MessageFormatter.PlayerJoin(activePlayer.Username, activePlayer.OnlineFrom.Value, activePlayer.PastOnline);
                await SendMessage(playerData);
            }

            var logs = MessageFormatter.Log(mcServer.Logs.TakeLast(50));
            await SendMessage(logs);
        }




        private async Task ReceiveDataAsync()
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
                            await MessageReceived(res);
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

        public async Task SendMessage(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

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

        private class WebSocketData
        {
            public string Request { get; set; }
            public string Data { get; set; }
        }
    }
}
