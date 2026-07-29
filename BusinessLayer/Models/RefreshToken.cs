using System;

namespace TicketResolver.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
