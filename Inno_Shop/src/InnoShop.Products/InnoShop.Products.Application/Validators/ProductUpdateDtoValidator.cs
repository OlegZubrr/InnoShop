using FluentValidation;
using InnoShop.Products.Application.DTOs;

namespace InnoShop.Products.Application.Validators;

public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name cannot be empty")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThan(1000000).WithMessage("Price must not exceed 1,000,000");
        });
    }
}