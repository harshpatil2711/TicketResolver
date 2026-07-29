using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class HistoryDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public List<TicketStatusHistory> GetByTicketId(int ticketId)
        {
            var list = new List<TicketStatusHistory>();
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketStatusHistoryGetByTicketId"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new TicketStatusHistory
                        {
                            HistoryId = Convert.ToInt32(row["HistoryId"]),
                            TicketId = Convert.ToInt32(row["TicketId"]),
                            OldStatusId = row["OldStatusId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["OldStatusId"]),
                            NewStatusId = Convert.ToInt32(row["NewStatusId"]),
                            ChangeReason = row["ChangeReason"] == DBNull.Value ? null : row["ChangeReason"].ToString(),
                            CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
