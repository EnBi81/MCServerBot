using MCWebServer.MinecraftServer;
using MCWebServer.PermissionControll;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using MCWebServer.Log;

namespace MCWebServer.WebSocketHandler
{
    public static class SocketPool
    {
        private static Dictionary<string, List<MCWebSocket>> Sockets { get; } = new Dictionary<string, List<MCWebSocket>>();
        private static HashSet<MCWebSocket> AllSockets { get; } = new HashSet<MCWebSocket>();

        static SocketPool()
        {
            LogService.GetService<WebLogger>().Log("socket-pool", "Socket Pool Initalizing");

            WebsitePermission.PermissionRemoved += async (sender, code) =>
            {
                string text = MessageFormatter.Logout();
                await BroadcastMessage(code, text);
               

                foreach (var item in Sockets[code])
                {
                    await item.Close();
                }
                
                RemoveCode(code);
            };

            ServerPark.Keklepcso.PlayerJoined += async (sender, player) =>
            {
                var mess = MessageFormatter.PlayerJoin(player.Username, player.OnlineFrom.Value, player.PastOnline);
                await BroadcastMessage(mess);
            };

            ServerPark.Keklepcso.PlayerLeft += async (sender, player) =>
            {
                var mess = MessageFormatter.PlayerLeft(player.Username);
                await BroadcastMessage(mess);
            };

            ServerPark.Keklepcso.LogReceived += async (sender, message) =>
            {
                var mess = MessageFormatter.Log(message.Message, type: (int)message.MessageType);
                await BroadcastMessage(mess);
            };

            ServerPark.Keklepcso.StatusChange += async (sender, status) =>
            {
                MinecraftServer.IMinecraftServer mcServer = (MinecraftServer.IMinecraftServer)sender;

                var mess = MessageFormatter.StatusUpdate(status, mcServer.OnlineFrom, mcServer.StorageSpace);
                await BroadcastMessage(mess);
            };

            ServerPark.Keklepcso.PerformanceMeasured += async (sender, performance) =>
            {
                var cpu = performance.CPU;
                var memory = performance.Memory;

                var mess = MessageFormatter.PcUsage(cpu, memory);
                await BroadcastMessage(mess);
            };

            LogService.GetService<WebLogger>().Log("socket-pool", "Socket Pool Initalized");
        }

        
        public static async Task AddSocket(string code, WebSocket socket)
        {
            var user = WebsitePermission.GetUser(code);
            LogService.GetService<WebLogger>().Log("socket-pool", "New socket received from " + user.Username);


            MCWebSocket socketHandler = new MCWebSocket(socket, user, code);
            RegisterSocket(code, socketHandler);

            await socketHandler.Initialize();
        }

        public static async Task BroadcastMessage(string message)
        {
            var sockets = GetSockets();
            foreach (var socket in sockets)
            {
                if(!socket.IsOpen)
                {
                    RemoveSocket(socket);
                    continue;
                }
                    

                try
                {
                    await socket.SendMessage(message);
                }
                catch { }
            }
        }

        public static async Task BroadcastMessage(string code, string message)
        {
            if (!Sockets.ContainsKey(code))
                return;


            var socketList = GetSockets(code);
            foreach (var socket in socketList)
            {
                if (!socket.IsOpen)
                {
                    RemoveSocket(socket);
                    continue;
                }
                try
                {
                    await socket.SendMessage(message);
                }
                catch { }
            }
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RegisterSocket(string code, MCWebSocket socket)
        {
            if(!Sockets.ContainsKey(code))
                Sockets.Add(code, new List<MCWebSocket> { socket });
            else
                Sockets[code].Add(socket);

            _ = AllSockets.Add(socket);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveSocket(MCWebSocket socket)
        {
            AllSockets.Remove(socket);
            if(Sockets.ContainsKey(socket.Code))
                Sockets[socket.Code].Remove(socket);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void RemoveCode(string code)
        {
            LogService.GetService<WebLogger>().Log("socket-pool", "Socket removed, code: " + code);

            foreach (MCWebSocket socket in AllSockets)
            {
                if(socket.Code == code)
                    AllSockets.Remove(socket);
            }

            Sockets.Remove(code);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEnumerable<MCWebSocket> GetSockets(string code)
        {
            return new List<MCWebSocket>(Sockets[code]);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEnumerable<MCWebSocket> GetSockets()
        {
            return new HashSet<MCWebSocket>(AllSockets);
        }
    }
}
