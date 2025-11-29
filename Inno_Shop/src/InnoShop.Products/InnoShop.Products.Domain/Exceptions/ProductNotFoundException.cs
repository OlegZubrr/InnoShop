namespace InnoShop.Products.Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(Guid productId)
        : base($"Product with ID {productId} was not found")
    {
    }
}