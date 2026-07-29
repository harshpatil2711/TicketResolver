using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace TicketResolver.DAL
{
    public class DashboardDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public DataSet GetStats(int? userId, int roleId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketDashboardStats"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@RoleId", DbType.Int32, roleId);
                return db.ExecuteDataSet(cmd);
            }
        }
    }
}
