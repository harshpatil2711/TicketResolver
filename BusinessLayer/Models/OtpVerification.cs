using System;

namespace TicketResolver.Models
{
    public class OtpVerification
    {
        public int OtpId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string Purpose { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsVerified { get; set; }
    }
}
