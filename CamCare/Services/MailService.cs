using CamCare.Interfaces.Services;
using CamCare.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CamCare.Services
{
    /// <summary>
    /// Versendet E-Mails als HTML über den konfigurierten SMTP-Server (MailKit).
    /// </summary>
    public class MailService(ILogger<MailService> logger) : IMailService
    {
        public async Task<ServiceResponse<bool>> SendAsync(
            MailSettingsVm settings,
            IEnumerable<string> recipients,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            var recipientList = (recipients ?? [])
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            try
            {
                if (string.IsNullOrWhiteSpace(settings.SmtpHost))
                    return ServiceResponse.Failure<bool>("Es ist kein SMTP-Server konfiguriert.");

                if (string.IsNullOrWhiteSpace(settings.FromAddress))
                    return ServiceResponse.Failure<bool>("Es ist keine Absenderadresse konfiguriert.");

                if (recipientList.Count == 0)
                    return ServiceResponse.Failure<bool>("Es ist keine Empfängeradresse vorhanden.");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.FromName?.Trim() ?? string.Empty, settings.FromAddress.Trim()));
                foreach (var recipient in recipientList)
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }

                message.Subject = subject ?? string.Empty;
                message.Body = new BodyBuilder { HtmlBody = htmlBody ?? string.Empty }.ToMessageBody();

                using var client = new SmtpClient
                {
                    Timeout = Math.Clamp(settings.TimeoutSeconds, 5, 300) * 1000
                };

                await client.ConnectAsync(settings.SmtpHost.Trim(), settings.SmtpPort, GetSocketOptions(settings.Security), cancellationToken);

                if (!string.IsNullOrWhiteSpace(settings.UserName))
                {
                    await client.AuthenticateAsync(settings.UserName.Trim(), settings.Password ?? string.Empty, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                logger.LogInformation("E-Mail \"{Subject}\" an {Recipients} versendet.", message.Subject, string.Join(", ", recipientList));
                return ServiceResponse.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Versenden der E-Mail \"{Subject}\" an {Recipients}.", subject, string.Join(", ", recipientList));
                return ServiceResponse.Failure<bool>($"Fehler beim Versenden der E-Mail: {ex.Message}", ex);
            }
        }

        private static SecureSocketOptions GetSocketOptions(MailSecurityMode security) => security switch
        {
            MailSecurityMode.None => SecureSocketOptions.None,
            MailSecurityMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
            _ => SecureSocketOptions.StartTls
        };
    }
}
