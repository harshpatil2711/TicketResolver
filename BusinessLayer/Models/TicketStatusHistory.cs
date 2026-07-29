using System;

namespace TicketResolver.Models
{
    public class TicketStatusHistory
    {
        public int HistoryId { get; set; }
        public int TicketId { get; set; }
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public int ChangedBy { get; set; }
        public DateTime ChangedDate { get; set; }
        public string Remarks { get; set; }
    }
}
