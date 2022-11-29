using System.IO.Compression;

namespace Application.Minecraft.Util
{
    internal class FileHelper
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

        public static void ZipDirectory(string sourceDir, string destinationZip)
        {
            ZipFile.CreateFromDirectory(sourceDir, destinationZip);
        }

        /// <summary>
        /// Calculates the size of a directory
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo? d)
        {
            if (d == null)
                return 0;

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
        public static string StorageFormatter(long size)
        {
            string measurement = "B";
            double temp = size;

            if (temp > 1024)
            {
                temp /= 1024;
                measurement = "KB";

                if (temp > 1024)
                {
                    temp /= 1024;
                    measurement = "MB";

                    if (temp > 1024)
                    {
                        temp /= 1024;
                        measurement = "GB";
                    }
                }
            }

            return Math.Round(temp, 2) + " " + measurement;
        }


        /// <summary>
        /// Checks if the directory's storage space in bytes is larger than the value specified in the maxBytes
        /// </summary>
        /// <param name="dir">Directory to check</param>
        /// <param name="maxBytes">Maximum allowed bytes</param>
        /// <returns>True if the storage has overflew.</returns>
        public static (bool Overflow, long Measured) CheckStorageOverflow(string dir, long maxBytes)
        {
            DirectoryInfo info = new(dir);
            long dirSize = DirSize(info);
            return (dirSize > maxBytes, dirSize);
        }

        internal static void DeleteDirectory(string newDir) => Directory.Delete(newDir, true);


        /// <summary>
        /// Creates a zip file from the given directory, including a filter by entry.
        /// </summary>
        /// <param name="sourceDirectoryName"></param>
        /// <param name="destinationArchiveFileName"></param>
        /// <param name="compressionLevel"></param>
        /// <param name="includeBaseDirectory"></param>
        /// <param name="filterEntryNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task CreateZipFromDirectory(
            string sourceDirectoryName, 
            string destinationArchiveFileName, 
            CompressionLevel compressionLevel, 
            bool includeBaseDirectory, 
            Predicate<string> filterEntryNames)
        {
            if (string.IsNullOrEmpty(sourceDirectoryName))
            {
                throw new ArgumentNullException(nameof(sourceDirectoryName));
            }
            if (string.IsNullOrEmpty(destinationArchiveFileName))
            {
                throw new ArgumentNullException(nameof(destinationArchiveFileName));
            }

            var filesToAdd = Directory.GetFiles(sourceDirectoryName, "*", SearchOption.AllDirectories);
            var entryNames = GetEntryNames(filesToAdd, sourceDirectoryName, includeBaseDirectory);
            await using var zipFileStream = new FileStream(destinationArchiveFileName, FileMode.Create);
            using var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create);

            foreach (var (entryName, fileName) in entryNames.Zip(filesToAdd))
            {
                if (filterEntryNames(entryName))
                {
                    archive.CreateEntryFromFile(fileName, entryName, compressionLevel);
                }
            }
        }
        private static string[] GetEntryNames(string[] names, string sourceFolder, bool includeBaseName)
        {
            if (names == null || names.Length == 0)
                return Array.Empty<string>();

            if (includeBaseName)
                sourceFolder = Path.GetDirectoryName(sourceFolder)!;

            int length = string.IsNullOrEmpty(sourceFolder) ? 0 : sourceFolder.Length;
            if (length > 0 && sourceFolder != null && sourceFolder[length - 1] != Path.DirectorySeparatorChar && sourceFolder[length - 1] != Path.AltDirectorySeparatorChar)
                length++;

            var result = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = names[i][length..];
            }

            return result;
        }
    }
}
