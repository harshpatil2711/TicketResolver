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
            var smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
            var smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
            var smtpFrom = ConfigurationManager.AppSettings["SmtpFrom"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;

                var message = new MailMessage(smtpFrom, toEmail, subject, body);
                message.IsBodyHtml = true;
                client.Send(message);
            }
        }
    }
}
