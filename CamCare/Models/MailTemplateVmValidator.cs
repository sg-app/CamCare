using FluentValidation;

namespace CamCare.Models
{
    public class MailTemplateVmValidator : AbstractValidator<MailTemplateVm>
    {
        public MailTemplateVmValidator()
        {
            RuleFor(t => t.RepairOrderStatusId)
                .GreaterThan(0)
                .WithMessage("Bitte den Status auswählen, bei dem die E-Mail versendet werden soll.");

            // Betreff und Inhalt sind nur erforderlich, wenn die Vorlage aktiv ist.
            RuleFor(t => t.SubjectTemplate)
                .NotEmpty().WithMessage("Der Betreff der E-Mail darf nicht leer sein.")
                .MaximumLength(300).WithMessage("Der Betreff darf maximal 300 Zeichen enthalten.")
                .When(t => t.IsEnabled);

            RuleFor(t => t.BodyTemplate)
                .NotEmpty().WithMessage("Der Inhalt der E-Mail darf nicht leer sein.")
                .When(t => t.IsEnabled);
        }
    }
}
