using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Infrastructure;
using FluentValidation;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Kernel.Domain.DTOs.Identity;

namespace ManagementBE.Presentation.Application.Validators.Forms
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
