using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MCWebServer.MinecraftServer
{
    internal class MinecraftServer
    {
        public string ServerName { get; }
        public string Address { get; }
        public int Port => int.Parse(Properties["server-port"]);
        public List<string> Log { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();

        private ServerStatus _status = ServerStatus.Offline;
        public ServerStatus Status
        {
            get => _status; 
            private set
            {
                _status = value;
                RaiseEvent(StatusChange, _status);
            }
        }
        public MinecraftServerProperties Properties { get; }
        public HashSet<string> OnlinePlayers { get; } = new HashSet<string>();

        private Process? _process;
        private readonly string _fileName;



        public MinecraftServer(string serverName, string address, string filename, string properties)
        {
            ServerName = serverName;
            Address = address;
            _fileName = filename;
            Properties = MinecraftServerProperties.GetProperties(properties);
        }




        public void Start(string user = "Admin")
        {
            if (IsRunning())
                throw new Exception("Server is already running");


            Status = ServerStatus.Starting;
            AddLog(user + ": " + "Starting Server");

            Hamachi.HamachiClient.LogOn();

            var workingDir = _fileName.Contains('/') 
                ? _fileName[.._fileName.LastIndexOf('/')] 
                : _fileName[.._fileName.LastIndexOf('\\')];

            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = _fileName,
                WorkingDirectory = workingDir
            };
            _process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            _process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    Errors.Add(e.Data);
                    RaiseEvent(ErrorEvent, e.Data);
                }
            };
            _process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    AddLog(e.Data);
                }
            };
            _process.Exited += (s, e) =>
            {
                Status = ServerStatus.Offline;
                _process = null;
            };
        }


        protected void AddLog(string log)
        {
            string baseTimeRegex = "\\[(\\d{2}:){2}\\d{2}\\] \\[Server thread\\/INFO\\]: ";
            Regex startupDoneRegex = new(baseTimeRegex + "Done \\([\\d.s]+\\)! For help, type \"help\"");
            Regex playerJoinedRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) joined the game");
            Regex playerLeftRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) left the game");
            Regex shutdownRegex = new(baseTimeRegex + "Stopping the server");


            Log.Add(log);
            RaiseEvent(LogReceived, log);

            // [14:02:39] [Server thread/INFO]: Done (44.552s)! For help, type "help"
            if (startupDoneRegex.IsMatch(log))
                Status = ServerStatus.Online;

            // [21:34:35] [Server thread/INFO]: Enbi81 joined the game
            else if (playerJoinedRegex.IsMatch(log))
            {
                var cap = playerJoinedRegex.Match(log).Captures[0];
                OnlinePlayers.Add(cap.Value);
                RaiseEvent(OnlinePlayerChange, OnlinePlayers.Count);
            }

            // [21:35:08] [Server thread/INFO]: Enbi81 left the game
            else if (playerLeftRegex.IsMatch(log))
            {
                var cap = playerLeftRegex.Match(log).Captures[0];
                OnlinePlayers.Remove(cap.Value);
                RaiseEvent(OnlinePlayerChange, OnlinePlayers.Count);
            }

            else if (shutdownRegex.IsMatch(log))
            {
                Status = ServerStatus.ShuttingDown;
            }
        }

        public void WriteCommand(string command, string user = "Admin")
        {
            try
            {
                AddLog(user + ": " + command);
                _process.StandardInput.WriteLine("stop");
            }
            catch
            {
                Errors.Add("Server is not online!");
            }
            
        }

        public bool IsRunning()
        {
            return Status == ServerStatus.Online 
                || Status == ServerStatus.Starting 
                || Status == ServerStatus.ShuttingDown;
        }

        public void Shutdown(string user = "Admin")
        {
            if (Status == ServerStatus.Offline)
                throw new Exception("Server is offline");
            if (Status == ServerStatus.ShuttingDown)
                throw new Exception("Server is already shutting down");
            if (Status == ServerStatus.Starting)
                throw new Exception("Server is Starting, please wait till it is online");


            try
            {
                WriteCommand("stop", user);
                
            }
            catch (Exception ex)
            {
                RaiseEvent(ErrorEvent, ex.Message);
            }

            Hamachi.HamachiClient.LogOff();
        }



        public event EventHandler<ServerStatus> StatusChange;
        public event EventHandler<string?> LogReceived;
        public event EventHandler<string?> ErrorEvent;
        public event EventHandler<int> OnlinePlayerChange;

        protected void RaiseEvent<T>(EventHandler<T>? evt, T param)
        {
            evt?.Invoke(this, param);
        }
    }
}
