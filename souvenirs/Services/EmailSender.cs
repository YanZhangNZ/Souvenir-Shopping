using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace souvenirs.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mes = new MimeMessage();
            mes.From.Add(new MailboxAddress("Yan", "zhangy337@myunitec.ac.nz"));
            mes.To.Add(new MailboxAddress("User", email));
            mes.Subject = subject;

            mes.Body = new TextPart("html")
            {
                Text = message
            };
            using(var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s,c,h,e) => true;
                client.Connect("smtp.office365.com", 587, false);

                //we don't have OAuth2 token so disable
                //the XOUTH2 authentication mechanism
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                //only needed if the smtp server requires authentication
                client.Authenticate("zhangy337@myunitec.ac.nz", "zhangyan100");
                client.Send(mes);
                client.Disconnect(true);
            }
            return Task.CompletedTask;
        }
    }
}
