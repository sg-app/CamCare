using FluentValidation;

namespace CamCare.Models
{
    public class RepairOrderVmValidator : AbstractValidator<RepairOrderVm>
    {
        public RepairOrderVmValidator()
        {
            RuleFor(r => r.CustomerId)
                .NotEmpty()
                .WithMessage("Kunde darf nicht leer sein.");

            RuleFor(r => r.RepairOrderStatusId)
                .GreaterThan(0)
                .WithMessage("Bearbeitungsstatus muss ausgewählt sein.");

            RuleFor(r => r.LogisticProviderId)
                .NotNull()
                .GreaterThan(0)
                .When(r => r.ShippingMethod == ShippingMethod.PickedUp)
                .WithMessage("Logistikdienstleister muss ausgewählt sein, wenn die Kamera abgeholt wurde.");

            RuleFor(r => r.PackagingHeight)
                .NotNull()
                .WithMessage("Kartonhöhe muss angegeben sein, wenn ein neuer Karton verwendet wird.")
                .GreaterThan(0)
                .WithMessage("Kartonhöhe muss größer als 0 sein, wenn ein neuer Karton verwendet wird.")
                .When(r => r.PackagingType == PackagingType.NewBox);

            RuleFor(r => r.PackagingLength)
                .NotNull()
                .WithMessage("Kartonlänge muss angegeben sein, wenn ein neuer Karton verwendet wird.")
                .GreaterThan(0)
                .WithMessage("Kartonlänge muss größer als 0 sein, wenn ein neuer Karton verwendet wird.")
                .When(r => r.PackagingType == PackagingType.NewBox);

            RuleFor(r => r.PackagingWidth)
                .NotNull()
                .WithMessage("Kartonbreite muss angegeben sein, wenn ein neuer Karton verwendet wird.")
                .GreaterThan(0)
                .WithMessage("Kartonbreite muss größer als 0 sein, wenn ein neuer Karton verwendet wird.")
                .When(r => r.PackagingType == PackagingType.NewBox);
        }
    }
}
