namespace TicketResolver.Models
{
    public class TicketStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsTerminalState { get; set; }
    }
}
