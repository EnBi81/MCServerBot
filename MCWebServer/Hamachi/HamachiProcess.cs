using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.Hamachi
{
    static class HamachiProcess
    {
        public static List<string> RequestData(string? command)
        {
            string text = "hamachi-2.exe --cli" + (command != null ? " " + command : "") + " & exit";

            var info = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                WorkingDirectory = Config.Config.Instance.HamachiLocation,
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
