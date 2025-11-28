namespace InnoShop.Users.Domain.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists")
    {
        Email = email;
    }

    public string Email { get; }
}