using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.MinecraftServers.Utils
{
    /// <summary>
    /// Creates the file system of a server if it is not created
    /// </summary>
    public class McServerFileStructure
    {
        /*
         * Structure:
         * 
         * 1/
         *   - server.info
         *   - server-files/
         *   - temp/
         *      - trash/
	     *      - backup/
         * 
         */




        public string RootFolder { get; }
        public string ServerFiles { get; }
        public string TempBackup { get; }
        public string TempTrash { get; }


        public McServerFileStructure(string rootFolder)
        {
            RootFolder = rootFolder;
            ServerFiles = $"{rootFolder}\\serverfiles";
            TempBackup = $"{rootFolder}\\temp\\backup";
            TempTrash = $"{rootFolder}\\temp\\trash";
            
            Directory.CreateDirectory(RootFolder);
            Directory.CreateDirectory(ServerFiles);
            Directory.CreateDirectory(TempBackup);
            Directory.CreateDirectory(TempTrash);
        }
    }
}
