namespace InnoShop.Users.Application.Events;

public class UserDeactivatedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime DeactivatedAt { get; set; }
}