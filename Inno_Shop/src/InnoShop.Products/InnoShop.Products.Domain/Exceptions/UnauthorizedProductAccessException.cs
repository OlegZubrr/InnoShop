namespace InnoShop.Products.Domain.Exceptions;

public class UnauthorizedProductAccessException : Exception
{
    public UnauthorizedProductAccessException()
        : base("You do not have permission to perform this operation on this product")
    {
    }
}