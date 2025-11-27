namespace InnoShop.Users.Domain.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException(string tokenType)
        : base($"Invalid or expired {tokenType} token")
    {
    }
}