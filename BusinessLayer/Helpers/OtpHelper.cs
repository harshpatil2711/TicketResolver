using System;
using System.Security.Cryptography;

namespace TicketResolver.Helpers
{
    public static class OtpHelper
    {
        public static string GenerateOtp(int length = 6)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);

                var otp = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    otp += (bytes[i] % 10).ToString();
                }
                return otp;
            }
        }
    }
}
