using System;
using Serilog;
using TicketResolver.DAL;

namespace TicketResolver.Helpers
{
    public static class AppLogger
    {
        private static readonly LogDAL logDAL = new LogDAL();

        public static void Error(string source, string message, Exception ex = null)
        {
            Log.Error(ex, "{Source}: {Message}", source, message);
            try { logDAL.Insert("ERROR", source, message, ex?.ToString(), ex?.StackTrace); } catch { }
        }

        public static void Warning(string source, string message)
        {
            Log.Warning("{Source}: {Message}", source, message);
            try { logDAL.Insert("WARNING", source, message); } catch { }
        }

        public static void Information(string source, string message)
        {
            Log.Information("{Source}: {Message}", source, message);
            try { logDAL.Insert("INFO", source, message); } catch { }
        }
    }
}