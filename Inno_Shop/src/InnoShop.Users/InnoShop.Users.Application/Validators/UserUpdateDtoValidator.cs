using FluentValidation;
using InnoShop.Users.Application.DTOs;

namespace InnoShop.Users.Application.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(3).WithMessage("First Name must be at least 3 characters long")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("User status is required");
    }
}