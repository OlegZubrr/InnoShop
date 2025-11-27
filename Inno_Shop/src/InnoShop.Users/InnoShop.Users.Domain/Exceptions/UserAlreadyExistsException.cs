namespace InnoShop.Users.Domain.Exceptions;

public class UserAlreadyExistsException:Exception
{
    public string Email { get; }

    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.")
    {
        Email = email;
    }
}