using System.IO.Compression;

namespace Sandbox
{

    public class Person
    {

        public string Name = "";
        public int Age { get; set; }
    }
    
    public class SandBoxClass
    {


        static void Main(string[] args)
        {
            Person p = new Person { Name = "Greg", Age = 20 };

            Console.WriteLine(p.Name);

            ref string name = ref p.Name;
            name = "Hello";

            Console.WriteLine(p.Name);
        }
           


        static async Task Main1(string[] args)
        {
            string fromDir = @"C:\Users\enbi8\source\repos\MCServerBot\code\Sandbox\bin\Debug\net7.0\1";
            string destination = @"1.zip";

            await CreateFromDirectory(fromDir, destination, CompressionLevel.SmallestSize, false, text => !text.StartsWith("eula.txt") && !text.StartsWith("logs"));
        }


        public static async Task CreateFromDirectory(
            string sourceDirectoryName
    ,       string destinationArchiveFileName
    ,       CompressionLevel compressionLevel
    ,       bool includeBaseDirectory
    ,       Predicate<string> filterEntryNames
    )
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
                return new string[0];

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

