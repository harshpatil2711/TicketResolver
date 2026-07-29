using System;

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
    }
}
