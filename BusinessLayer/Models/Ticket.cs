using System;

namespace TicketResolver.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
