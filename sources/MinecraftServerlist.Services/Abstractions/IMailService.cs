namespace MinecraftServerlist.Services.Abstractions;

public interface IMailService
{
    public Task SendMailAsync(string[] to, string[] cc, string[] bcc, string subject, string content, CancellationToken cancellationToken = default);
}