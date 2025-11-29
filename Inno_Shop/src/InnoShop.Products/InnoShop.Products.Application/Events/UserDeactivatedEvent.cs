namespace InnoShop.Products.Application.Events;

public class UserDeactivatedEvent
{
    public Guid UserId { get; set; }
    public DateTime DeactivatedAt { get; set; }
}