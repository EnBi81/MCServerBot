using Loggers;
using Application.PermissionControll;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.WebSocketHandler
{
    /// <summary>
    /// Handles a single websocket instance.
    /// </summary>
    public class MCWebSocket
    {
        private readonly WebSocket _socket;
        private bool _readInput = true; // true if it should read input from the socket, false if not.

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
            _socket = socket;
            DiscordUser = discordUser;
            Code = code;

            LogService.GetService<WebLogger>().Log("socket", $"Socket Initialized for {DiscordUser.Username}");
        }


        /// <summary>
        /// Sends an error message to the socket.
        /// </summary>
        /// <param name="message">error message</param>
        /// <returns></returns>
        public async Task SendBackErrorMessage(string message)
        {
            LogService.GetService<WebLogger>().Log("socket", $"Error from {DiscordUser.Username}, data: {message}");

            string mess = MessageFormatter.ErrorMessage(message);
            await SendMessage(mess);
        }



        #region Low level network stuff, no business logic


        /// <summary>
        /// Sends a text message through the socket.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SendMessage(string text)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await _socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                await Close();
            }
            
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

        #endregion
    }
}
