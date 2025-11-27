using FluentValidation;
using InnoShop.Users.Application.DTOs;

namespace InnoShop.Users.Application.Validators;

public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleDtoValidator()
    {
        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}