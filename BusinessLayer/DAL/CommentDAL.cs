using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class CommentDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public int Insert(int ticketId, int userId, string commentText, bool isInternalNote)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketCommentInsert"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@CommentText", DbType.String, commentText);
                db.AddInParameter(cmd, "@IsInternalNote", DbType.Boolean, isInternalNote);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public List<TicketComment> GetByTicketId(int ticketId, int userId, int roleId)
        {
            var list = new List<TicketComment>();
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketCommentGetByTicketId"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@RoleId", DbType.Int32, roleId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new TicketComment
                        {
                            CommentId = Convert.ToInt32(row["CommentId"]),
                            TicketId = Convert.ToInt32(row["TicketId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            UserName = row["UserName"].ToString(),
                            RoleName = row["RoleName"].ToString(),
                            CommentText = row["CommentText"].ToString(),
                            IsInternalNote = Convert.ToBoolean(row["IsInternalNote"]),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
