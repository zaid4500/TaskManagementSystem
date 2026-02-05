using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using ManagementBE.Kernel.Domain.Identity;

namespace ManagementBE.Presentation.Application.Validators.Identity
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FirstName)
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.FirstNameAr)
                .NotEmpty().WithMessage("First name in Arabic is required")
                .MaximumLength(100).WithMessage("First name in Arabic must not exceed 100 characters");

            RuleFor(x => x.LastNameAr)
                .NotEmpty().WithMessage("Last name in Arabic is required")
                .MaximumLength(100).WithMessage("Last name in Arabic must not exceed 100 characters");

            RuleFor(x => x.GenderId)
                .InclusiveBetween(1, 2).When(x => x.GenderId.HasValue)
                .WithMessage("Gender must be 1 (Male) or 2 (Female)");

            RuleFor(x => x.Nationality)
                .MaximumLength(100).WithMessage("Nationality must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format")
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

            RuleFor(x => x.MobileNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.MobileNumber))
                .WithMessage("Invalid mobile number format")
                .MaximumLength(20).WithMessage("Mobile number must not exceed 20 characters");
        }
    }
}
