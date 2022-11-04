﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Sandbox
{
    public class SandBoxClass
    {
        public struct S
        {
            private int? num;

            public int Num => num ?? 43;

            public S()
            {
                num = null;
            }
        }


        const string Server_Jar = "A:\\mc-server-test\\server.jar";
        const string Server_Folder = "A:\\mc-server-test\\server1\\";
        static async Task Main(string[] args) // this is how to create a new server
        {

            S val = default;
            Console.WriteLine(val.Num);


            //if(args.Contains("reset"))
            //{
            //    Console.WriteLine("Deleting " + Server_Folder);
            //    try
            //    {
            //        Directory.Delete(Server_Folder, true);
            //        Console.WriteLine("Delete done");
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e.Message);
            //    }
                
            //    return;
            //}

            //var serverFile = new FileInfo(Server_Jar);

            //CreateDirAndCopyFile(Server_Folder, serverFile);
            //RunStartupServerFile(serverFile, Server_Folder);
            //AcceptEula(new FileInfo(Server_Folder + "eula.txt"));
            //RunStartupServerFile(serverFile, Server_Folder);
        }

        static void CreateDirAndCopyFile(string dir, FileInfo file)
        {
            Console.WriteLine("Creating directory " + dir);
            Directory.CreateDirectory(dir);
        }

        static void RunStartupServerFile(FileInfo info, string dir)
        {
            string dirPath = dir;
            string filename = info.Name;

            Console.WriteLine("Starting process");

            Process? p = Process.Start(new ProcessStartInfo
            {
                FileName = "C:\\Program Files\\Java\\jdk-17.0.2\\bin\\java.exe",
                WorkingDirectory = dirPath,
                CreateNoWindow = true,
                Arguments = "-jar " + info.FullName,
            });
            p.OutputDataReceived += (s, d) => Console.WriteLine(d.Data);

            p.WaitForExit();
        }

        static void AcceptEula(FileInfo eula)
        {
            string fullPath = eula.FullName;
            Console.WriteLine("Reading eula");
            string text = File.ReadAllText(fullPath);
            var newEula = text.Replace("eula=false", "eula=true");
            File.WriteAllText(fullPath, newEula);
            Console.WriteLine("New eula created");
        }
    }
}

