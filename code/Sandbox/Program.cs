using System.Net;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            var fileInfo = new FileInfo("test.txt");
            Console.WriteLine("Edited time: " + fileInfo.LastWriteTimeUtc);

            Console.WriteLine("Waiting for enter...");
            Console.ReadLine();

            fileInfo.Refresh();
            Console.WriteLine("Edited time: " + fileInfo.LastWriteTimeUtc);
        }
    }
}

