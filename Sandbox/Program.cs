using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Sandbox
{
    public class SandBoxClass
    {
        static void Main(string[] args)
        {
            var dic = new Dictionary<string, object>()
            {
                ["text"] = "hi",
                ["number"] = 2,
                ["dic"] = new Dictionary<string, object?>
                {
                    ["double"] = 2.2,
                    ["null"] = null
                }
            };


            string text = JsonConvert.SerializeObject(dic);

            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text) ?? new Dictionary<string, object>();

            Console.WriteLine(data["text"]);
            Console.WriteLine(data["number"].GetType().FullName);
            if(data["dic"] is JObject obj)
                Console.WriteLine(obj["double"]);

        }
    }
}

