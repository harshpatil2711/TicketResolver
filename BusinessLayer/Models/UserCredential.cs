using System;

namespace TicketResolver.Models
{
    public class UserCredential
    {
        public int CredentialId { get; set; }
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
