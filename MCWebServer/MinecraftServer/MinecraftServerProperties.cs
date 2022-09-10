using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MCWebServer.MinecraftServer
{
    public class MinecraftServerProperties
    {
        public static MinecraftServerProperties GetProperties(string file)
        {
            string[] lines = File.ReadAllLines(file);
            return new MinecraftServerProperties(lines);
        }

        public static void SaveProperties(string file, MinecraftServerProperties props)
        {
            string text = "";
            foreach(var (key, value) in props)
                text += key + "=" + value.ToString() + Environment.NewLine;

            File.WriteAllText(file, text);
        }

        

        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public MinecraftServerProperties(IEnumerable<string> lines)
        {
            Regex regex = new Regex("[^=]=[^=]");
            foreach(var line in lines)
            {
                if(!regex.IsMatch(line))
                    continue;

                string[] parts = line.Split('=');
                string key = parts[0];
                string value = parts[1];

                Properties.Add(key, value);
            }
        }

        public string this[string key]
        {
            get => Properties[key];
            set => Properties[key] = value;
        }

        public Dictionary<string, string>.Enumerator GetEnumerator()
        {
            return Properties.GetEnumerator();
        }
    }
}
