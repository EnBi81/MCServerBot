using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    public static class ControllerUtils
    {
        public static object FormatErrorMessage(this ControllerBase cBase, string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }
    }
}
