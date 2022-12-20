using Application.Minecraft.MinecraftServers.Utils;
using System.Drawing;
using System.IO.Compression;

namespace Sandbox
{
    public class SandBoxClass
    {

        static void Main(string[] args)
        {
            var f = "C:\\Users\\enbi8\\Downloads\\bricks.png";
            
            var file = File.Open(f, FileMode.Open);
            Bitmap.FromStream(file);
        }
    }
}

