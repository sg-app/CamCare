using FluentValidation;

namespace CamCare.Models
{
    public class DeliveryAddressVmValidator : AbstractValidator<DeliveryAddressVm>
    {
        public DeliveryAddressVmValidator()
        {
            RuleFor(a => a.CustomerId)
                .GreaterThan(0)
                .WithMessage("Die Lieferanschrift muss einem Kunden zugeordnet sein.");

            RuleFor(a => a.Recipient)
                .NotEmpty()
                .WithMessage("Empfänger darf nicht leer sein.")
                .MaximumLength(200)
                .WithMessage("Empfänger darf maximal 200 Zeichen enthalten.");

            RuleFor(a => a.Street)
                .NotEmpty()
                .WithMessage("Straße darf nicht leer sein.")
                .MaximumLength(200)
                .WithMessage("Straße darf maximal 200 Zeichen enthalten.");

            RuleFor(a => a.PostalCode)
                .NotEmpty()
                .WithMessage("PLZ darf nicht leer sein.")
                .MaximumLength(20)
                .WithMessage("PLZ darf maximal 20 Zeichen enthalten.");

            RuleFor(a => a.City)
                .NotEmpty()
                .WithMessage("Ort darf nicht leer sein.")
                .MaximumLength(200)
                .WithMessage("Ort darf maximal 200 Zeichen enthalten.");

            RuleFor(a => a.Country)
                .MaximumLength(100)
                .WithMessage("Land darf maximal 100 Zeichen enthalten.");

            RuleFor(a => a.ContactPerson)
                .MaximumLength(200)
                .WithMessage("Ansprechpartner darf maximal 200 Zeichen enthalten.");
        }
    }
}
