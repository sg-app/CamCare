using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CamCare.Services
{
    /// <summary>
    /// Versendet E-Mails als HTML über den in der appsettings.json konfigurierten
    /// SMTP-Server (MailKit).
    /// </summary>
    public class MailService(IOptions<MailOptions> mailOptions, ILogger<MailService> logger) : IMailService
    {
        private readonly MailOptions _options = mailOptions.Value;

        public async Task<ServiceResponse<bool>> SendAsync(
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
                if (string.IsNullOrWhiteSpace(_options.SmtpHost))
                    return ServiceResponse.Failure<bool>("Es ist kein SMTP-Server konfiguriert. Bitte den Abschnitt 'Mail' der appsettings.json prüfen.");

                if (string.IsNullOrWhiteSpace(_options.FromAddress))
                    return ServiceResponse.Failure<bool>("Es ist keine Absenderadresse konfiguriert. Bitte den Abschnitt 'Mail' der appsettings.json prüfen.");

                if (recipientList.Count == 0)
                    return ServiceResponse.Failure<bool>("Es ist keine Empfängeradresse vorhanden.");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_options.FromName?.Trim() ?? string.Empty, _options.FromAddress.Trim()));
                foreach (var recipient in recipientList)
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }

                message.Subject = subject ?? string.Empty;
                message.Body = new BodyBuilder { HtmlBody = htmlBody ?? string.Empty }.ToMessageBody();

                using var client = new SmtpClient
                {
                    Timeout = Math.Clamp(_options.TimeoutSeconds, 5, 300) * 1000
                };

                await client.ConnectAsync(_options.SmtpHost.Trim(), _options.SmtpPort, GetSocketOptions(_options.Security), cancellationToken);

                if (!string.IsNullOrWhiteSpace(_options.UserName))
                {
                    await client.AuthenticateAsync(_options.UserName.Trim(), _options.Password ?? string.Empty, cancellationToken);
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
