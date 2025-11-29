namespace InnoShop.Products.Application.Events;

public class UserActivatedEvent
{
    public Guid UserId { get; set; }
    public DateTime ActivatedAt { get; set; }
}