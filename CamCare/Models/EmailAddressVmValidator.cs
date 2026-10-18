using FluentValidation;

namespace CamCare.Models
{
    public class EmailAddressVmValidator : AbstractValidator<EmailAddressVm>
    {
        public EmailAddressVmValidator()
        {
            RuleFor(a => a.CustomerId)
                .GreaterThan(0)
                .WithMessage("Die E-Mail-Adresse muss einem Kunden zugeordnet sein.");

            RuleFor(a => a.Address)
                .NotEmpty()
                .WithMessage("E-Mail-Adresse darf nicht leer sein.")
                .MaximumLength(320)
                .WithMessage("E-Mail-Adresse darf maximal 320 Zeichen enthalten.")
                .EmailAddress()
                .WithMessage("E-Mail-Adresse ist nicht gültig.");
        }
    }
}
