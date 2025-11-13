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
        }
    }
}
