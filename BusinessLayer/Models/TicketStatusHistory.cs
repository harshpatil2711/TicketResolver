using System;

namespace TicketResolver.Models
{
    public class TicketStatusHistory
    {
        public int HistoryId { get; set; }
        public int TicketId { get; set; }
        public int? OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public int? PreviousAssignedTo { get; set; }
        public int? CurrentAssignedTo { get; set; }
        public string ChangeReason { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
