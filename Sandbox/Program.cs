using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sandbox
{
    public class SandBoxClass
    {
        static void Main(string[] args)
        {
            string text = ";";
            GetNumber(out int number, ref text);

            Console.WriteLine(number);

        }

        static void GetNumber(out int num, ref string text)
        {
            num = 2;

        }
    }
}

