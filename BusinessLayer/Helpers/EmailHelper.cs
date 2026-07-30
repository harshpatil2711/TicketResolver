using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace TicketResolver.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(string toEmail, string subject, string body)
        {
            var smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            var smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            var smtpSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpSsl"] ?? "true");
            var senderEmail = ConfigurationManager.AppSettings["SmtpSenderEmail"];
            var senderName = ConfigurationManager.AppSettings["SmtpSenderName"] ?? "Ticket Resolver";

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = smtpSsl;

                var fromAddress = new MailAddress(senderEmail, senderName);
                var message = new MailMessage(fromAddress, new MailAddress(toEmail))
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                client.Send(message);
            }
        }
    }
}
