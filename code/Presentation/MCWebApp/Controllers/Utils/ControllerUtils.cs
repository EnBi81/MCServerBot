using Application.Minecraft;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MCWebApp.Controllers.Utils
{
    public static class ControllerUtils
    {
        public static object FormatErrorMessage(this ControllerBase cBase, string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }

        public static string TryGetStringFromJson(Dictionary<string, object?> data, string key)
        {
            JsonValueKind? valueKind = null;
            if (data.TryGetValue(key, out object? temp) && // check if the jsonobject has this key, and get the value
                temp is JsonElement json &&   // check if the value is jsonelement (obviously it is, here we more just convert it to JsonElement)
                (valueKind = json.ValueKind) == JsonValueKind.String) // set the valuekind parameter to the received valuekind, and check if it is a string
            {
                string? value = json.ToString();

                if (value != null)
                    return value;
            }

            throw new Exception($"'{key}' value is expected to be a string, but was {valueKind}.");
        }
    }
}
