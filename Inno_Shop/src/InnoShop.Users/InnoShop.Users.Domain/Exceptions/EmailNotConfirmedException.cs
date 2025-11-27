namespace InnoShop.Users.Domain.Exceptions;

public class EmailNotConfirmedException : Exception
{
    public EmailNotConfirmedException()
        : base("Please confirm your email before logging in")
    {
    }
}