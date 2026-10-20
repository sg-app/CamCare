using FluentValidation;

namespace CamCare.Models
{
    public class MailSettingsVmValidator : AbstractValidator<MailSettingsVm>
    {
        public MailSettingsVmValidator()
        {
            RuleFor(s => s.SmtpHost)
                .NotEmpty().WithMessage("SMTP-Server darf nicht leer sein.")
                .MaximumLength(255).WithMessage("SMTP-Server darf maximal 255 Zeichen enthalten.");

            RuleFor(s => s.SmtpPort)
                .InclusiveBetween(1, 65535).WithMessage("Der SMTP-Port muss zwischen 1 und 65535 liegen.");

            RuleFor(s => s.TimeoutSeconds)
                .InclusiveBetween(5, 300).WithMessage("Das Zeitlimit muss zwischen 5 und 300 Sekunden liegen.");

            RuleFor(s => s.FromAddress)
                .NotEmpty().WithMessage("Absenderadresse darf nicht leer sein.")
                .MaximumLength(320).WithMessage("Absenderadresse darf maximal 320 Zeichen enthalten.")
                .EmailAddress().WithMessage("Absenderadresse ist nicht gültig.");

            RuleFor(s => s.FromName)
                .MaximumLength(255).WithMessage("Absendername darf maximal 255 Zeichen enthalten.");

            RuleFor(s => s.UserName)
                .MaximumLength(255).WithMessage("Benutzername darf maximal 255 Zeichen enthalten.");
        }
    }
}
