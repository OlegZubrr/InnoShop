namespace InnoShop.Users.Application.Events;

public class UserDeletedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}