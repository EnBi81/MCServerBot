using Shared.Exceptions;

namespace MCWebAPI.APIExceptions
{
    public class LoginException : MCExternalException
    {
        public LoginException(string? message = null) : base(message) { }
    }
}
