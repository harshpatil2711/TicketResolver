using System;

namespace TicketResolver.Models
{
    public class TicketAttachment
    {
        public int AttachmentId { get; set; }
        public int TicketId { get; set; }
        public int? CommentId { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
