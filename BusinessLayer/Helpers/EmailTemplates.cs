using System;
using System.IO;

namespace TicketResolver.Helpers
{
    public static class EmailTemplates
    {
        public static string PopulateOtpTemplate(string template, string otpCode, string purpose, string userName)
        {
            string purposeText = purpose == "Login" ? "signing in" : "registering";

            return template
                .Replace("{{userName}}", userName)
                .Replace("{{purposeText}}", purposeText)
                .Replace("{{otpCode}}", otpCode);
        }

        public static string PopulateNotificationTemplate(string template, string userName, string ticketNumber, string message, string actionBy)
        {
            return template
                .Replace("{{userName}}", userName)
                .Replace("{{ticketNumber}}", ticketNumber)
                .Replace("{{message}}", message)
                .Replace("{{actionBy}}", actionBy);
        }

        public static string LoadTemplate(string templatePath)
        {
            return File.ReadAllText(templatePath);
        }
    }
}
