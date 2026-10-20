using CamCare.Domain;
using CamCare.Models;

namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Verwaltet die E-Mail-Vorlagen, die beim Erreichen eines Status versendet werden.
    /// </summary>
    public interface IMailTemplateService : IDbService<MailTemplate, MailTemplateVm>
    {
    }
}
