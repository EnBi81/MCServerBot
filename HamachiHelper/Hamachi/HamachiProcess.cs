using System.Diagnostics;

namespace HamachiHelper
{
    /// <summary>
    /// Low level process handling for Hamachi.
    /// </summary>
    internal static class HamachiProcess
    {
        private static string _hamachiWorkingDir = "";
        public static void Setup(string hamachiWorkingDir)
        {
            _hamachiWorkingDir = hamachiWorkingDir;
        }

        /// <summary>
        /// Starts hamachi-2.exe program and enters the command for execution, then closes the client
        /// </summary>
        /// <param name="command">command to run</param>
        /// <returns>Output of hamachi client.</returns>
        public static List<string> RequestData(string? command)
        {
            string text = "hamachi-2.exe --cli" + (command != null ? " " + command : "") + " & exit";

            var info = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = _hamachiWorkingDir,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
            };

            Process hamachiProcess = new Process()
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };

            hamachiProcess.Start();
            hamachiProcess.BeginOutputReadLine();
            List<string> output = new List<string>();

            int i = 0;
            hamachiProcess.OutputDataReceived += (s, e) =>
            {
                if(e.Data != null && i++ > 3)
                    output.Add(e.Data);
            };

            hamachiProcess.StandardInput.WriteLine(text);

            hamachiProcess.WaitForExit();

            return output;
        }
    }
}
