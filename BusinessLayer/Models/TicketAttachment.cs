using System;

namespace TicketResolver.Models
{
    public class TicketAttachment
    {
        public int AttachmentId { get; set; }
        public int TicketId { get; set; }
        public int? CommentId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
