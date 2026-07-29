using System;

namespace TicketResolver.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogLevel { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
