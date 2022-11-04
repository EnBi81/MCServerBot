using Shared.Exceptions;

namespace APIModel.APIExceptions
{
    public class LoginException : MCExternalException
    {
        public LoginException(string? message = null) : base(message) { }
    }
}
