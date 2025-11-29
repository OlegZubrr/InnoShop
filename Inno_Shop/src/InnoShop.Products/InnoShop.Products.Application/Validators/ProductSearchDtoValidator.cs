using FluentValidation;
using InnoShop.Products.Application.DTOs;

namespace InnoShop.Products.Application.Validators;

public class ProductSearchDtoValidator : AbstractValidator<ProductSearchDto>
{
    public ProductSearchDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        When(x => x.MinPrice.HasValue, () =>
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum price cannot be negative");
        });

        When(x => x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum price cannot be negative");
        });

        When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x.MinPrice)
                .LessThanOrEqualTo(x => x.MaxPrice)
                .WithMessage("Minimum price must not exceed maximum price");
        });
    }
}