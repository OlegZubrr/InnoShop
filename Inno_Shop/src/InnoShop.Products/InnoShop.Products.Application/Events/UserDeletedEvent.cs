namespace InnoShop.Products.Application.Events;

public class UserDeletedEvent
{
    public Guid UserId { get; set; }
    public DateTime DeletedAt { get; set; }
}