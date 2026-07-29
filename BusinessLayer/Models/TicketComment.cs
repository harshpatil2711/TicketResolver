using System;

namespace TicketResolver.Models
{
    public class TicketComment
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string CommentText { get; set; }
        public bool IsInternalNote { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
