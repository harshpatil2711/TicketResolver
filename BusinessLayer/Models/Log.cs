using System;

namespace TicketResolver.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Source { get; set; }
        public int? UserId { get; set; }
        public DateTime LogDate { get; set; }
    }
}
