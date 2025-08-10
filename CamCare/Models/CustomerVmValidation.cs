using FluentValidation;

namespace CamCare.Models
{
    public class CustomerVmValidation : AbstractValidator<CustomerVm>
    {
        public CustomerVmValidation()
        {
            RuleFor(r => r.Id)
             .MinimumLength(1)
             .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.");

            RuleFor(r => r.CompanyName)
              .MinimumLength(1)
              .When(r => string.IsNullOrEmpty(r.FirstName) && string.IsNullOrEmpty(r.LastName))
              .WithName("Firmenname")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(200)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.FirstName)
              .MinimumLength(1)
              .When(r => string.IsNullOrEmpty(r.CompanyName))
              .WithName("Vorname")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(100)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.LastName)
              .MinimumLength(1)
              .When(r => string.IsNullOrEmpty(r.CompanyName))
              .WithName("Nachname")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(100)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.Email)
              .MinimumLength(4)
              .WithName("E-Mail")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(250)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.")
              .EmailAddress()
              .WithMessage("{PropertyName} hat kein gültiges Format.");

            RuleFor(r => r.PhoneNumber)
              .MinimumLength(1)
              .WithName("Telefonnummer")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(100)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleForEach(r => r.Addresses).SetValidator(new AddressVmValidation());
        }
    }
}
