namespace InnoShop.Users.Application.Events;

public class UserActivatedEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime ActivatedAt { get; set; }
}