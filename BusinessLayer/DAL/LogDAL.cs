using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace TicketResolver.DAL
{
    public class LogDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public void Insert(string logLevel, string source, string message, string exception = null, string stackTrace = null)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverLogInsert"))
            {
                db.AddInParameter(cmd, "@LogLevel", DbType.String, logLevel);
                db.AddInParameter(cmd, "@Source", DbType.String, source ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@Message", DbType.String, message);
                db.AddInParameter(cmd, "@Exception", DbType.String, exception ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@StackTrace", DbType.String, stackTrace ?? (object)DBNull.Value);
                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
