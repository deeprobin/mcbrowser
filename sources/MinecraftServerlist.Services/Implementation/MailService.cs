using MinecraftServerlist.Services.Abstractions;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace MinecraftServerlist.Services.Implementation;

internal sealed class MailService : IMailService
{
    private readonly SmtpClient _smtpClient;
    private readonly MailAddress _senderAddress;

    private readonly IMetricsService _metricsService;

    public MailService(SmtpClient smtpClient, IMetricsService metricsService)
    {
        _smtpClient = smtpClient;
        _metricsService = metricsService;

        // TODO: Make configurable
        _senderAddress = new MailAddress("noreply@mcbrowser.net", "MCBrowser.NET", Encoding.UTF8);
    }

    public async Task SendMailAsync(string[] recipients, string[] cc, string[] bcc, string subject, string content, CancellationToken cancellationToken = default)
    {
        var mailMessage = ConstructMailMessage(recipients, cc, bcc, subject, content);
        await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
        _metricsService.IncreaseMailCount();
    }

    private MailMessage ConstructMailMessage(ReadOnlySpan<string> recipients, ReadOnlySpan<string> cc, ReadOnlySpan<string> bcc, string subject,
        string content)
    {
        var mailMessage = new MailMessage
        {
            Body = content,
            BodyEncoding = Encoding.UTF8,
            BodyTransferEncoding = TransferEncoding.QuotedPrintable,
            DeliveryNotificationOptions = DeliveryNotificationOptions.None,
            From = _senderAddress,
            IsBodyHtml = true,
            Priority = MailPriority.Normal,
            Sender = _senderAddress,
            Subject = subject,
            SubjectEncoding = Encoding.UTF8
        };

        foreach (var recipientAddress in recipients)
        {
            if (!MailAddress.TryCreate(recipientAddress, out var recipientMailAddress)) continue;
            mailMessage.To.Add(recipientMailAddress);
        }

        foreach (var carbonCopyAddress in cc)
        {
            if (!MailAddress.TryCreate(carbonCopyAddress, out var carbonCopyMailAddress)) continue;
            mailMessage.CC.Add(carbonCopyMailAddress);
        }

        foreach (var blindCarbonCopyAddress in bcc)
        {
            if (!MailAddress.TryCreate(blindCarbonCopyAddress, out var blindCarbonCopyMailAddress)) continue;
            mailMessage.CC.Add(blindCarbonCopyMailAddress);
        }

        return mailMessage;
    }
}