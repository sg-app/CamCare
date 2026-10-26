using FluentValidation;

namespace CamCare.Models
{
    public class MailSignatureVmValidator : AbstractValidator<MailSignatureVm>
    {
        public MailSignatureVmValidator()
        {
            // Der Inhalt ist nur erforderlich, wenn die Signatur an E-Mails angehängt wird.
            RuleFor(s => s.HtmlTemplate)
                .NotEmpty().WithMessage("Der Inhalt der Signatur darf nicht leer sein.")
                .When(s => s.IsEnabled);

            RuleFor(s => s.LogoFileName)
                .MaximumLength(260).WithMessage("Der Dateiname des Logos darf maximal 260 Zeichen enthalten.");
        }
    }
}
