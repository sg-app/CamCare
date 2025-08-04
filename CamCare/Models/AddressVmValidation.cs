using FluentValidation;

namespace CamCare.Models
{
    public class AddressVmValidation : AbstractValidator<AddressVm>
    {
        public AddressVmValidation()
        {
            RuleFor(r => r.Street)
                .MinimumLength(5)
                .WithName("Straße")
                .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
                .MaximumLength(250)
                .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.HouseNumber)
                .MaximumLength(10)
                .WithName("Hausnummer")
                .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.PostalCode)
               .MinimumLength(4)
               .WithName("Postleitzahl")
               .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
               .MaximumLength(20)
               .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.City)
               .MinimumLength(4)
               .WithName("Stadt")
               .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
               .MaximumLength(100)
               .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

            RuleFor(r => r.Country)
              .MinimumLength(4)
              .WithName("Land")
              .WithMessage("{PropertyName} muss mindestens {MinLength} Zeichen lang sein.")
              .MaximumLength(100)
              .WithMessage("{PropertyName} darf maximal {MaxLength} Zeichen lang sein.");

        }
    }
}
