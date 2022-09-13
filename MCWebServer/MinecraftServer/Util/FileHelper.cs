using MCWebServer.Log;
using System;
using System.IO;

namespace MCWebServer.MinecraftServer.Util
{
    public class FileHelper
    {
        /// <summary>
        /// Moves a directory to the destination directory
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destinationDir"></param>
        public static void MoveDirectory(string sourceDir, string destinationDir)
        {
            Directory.Move(sourceDir, destinationDir);
        }


        /// <summary>
        /// Copy a folder and all its contents to a new location
        /// </summary>
        /// <param name="sourceDir">source directory</param>
        /// <param name="destinationDir">destination directory</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }

        /// <summary>
        /// Calculates the size of a directory
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        /// <summary>
        /// Formats the byte count into a human readable text
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string StorageFormatter(double size)
        {
            string measurement = "B";

            if (size > 1024)
            {
                size /= 1024;
                measurement = "KB";

                if (size > 1024)
                {
                    size /= 1024;
                    measurement = "MB";

                    if (size > 1024)
                    {
                        size /= 1024;
                        measurement = "GB";
                    }
                }
            }

            return Math.Round(size, 2) + " " + measurement;
        }
    }
}
