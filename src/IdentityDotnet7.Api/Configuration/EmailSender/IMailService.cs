namespace IdentityDotnet7.Api.Configuration.EmailSender;

public interface IMailService
{
    bool Send(string sender, string subject, string body, bool isBodyHTML);
}