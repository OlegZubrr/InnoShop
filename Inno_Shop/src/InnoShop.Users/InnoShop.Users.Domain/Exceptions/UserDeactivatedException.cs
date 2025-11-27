namespace InnoShop.Users.Domain.Exceptions;

public class UserDeactivatedException : Exception
{
    public UserDeactivatedException()
        : base("Your account has been deactivated. Please contact support.")
    {
    }
}