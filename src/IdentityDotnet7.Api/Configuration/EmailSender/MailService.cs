using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace IdentityDotnet7.Api.Configuration.EmailSender;

public class MailService : IMailService
{
    private readonly EmailSettings _emailSettings;

    public MailService(IOptions<EmailSettings> emailOptions)
    {
        _emailSettings = emailOptions.Value;
    }

    public bool Send(string sender, string subject, string body, bool isBodyHTML)
    {
        try
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailSettings.Email);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHTML;
            mailMessage.To.Add(new MailAddress(sender));

            var smtp = new SmtpClient();
            smtp.Host = "mail.teste.com";
            
            //If using gmail, set it as true
            smtp.EnableSsl = false;

            var networkCredential = new NetworkCredential();
            networkCredential.UserName = mailMessage.From.Address;
            networkCredential.Password = _emailSettings.Password;
            
            //If using gmail, set it as true
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            
            //If using google set it as 587
            smtp.Port = 25;
            
            smtp.Send(mailMessage);

            return true;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}