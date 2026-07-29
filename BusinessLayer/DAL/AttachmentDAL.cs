using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class AttachmentDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public int Insert(int ticketId, int? commentId, string originalFileName, string storedFileName, int createdBy)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAttachmentInsert"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@CommentId", DbType.Int32, commentId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@OriginalFileName", DbType.String, originalFileName);
                db.AddInParameter(cmd, "@StoredFileName", DbType.String, storedFileName);
                db.AddInParameter(cmd, "@CreatedBy", DbType.Int32, createdBy);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public TicketAttachment GetById(int attachmentId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAttachmentGetById"))
            {
                db.AddInParameter(cmd, "@AttachmentId", DbType.Int32, attachmentId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count == 0) return null;
                    DataRow row = ds.Tables[0].Rows[0];
                    return new TicketAttachment
                    {
                        AttachmentId = Convert.ToInt32(row["AttachmentId"]),
                        TicketId = Convert.ToInt32(row["TicketId"]),
                        CommentId = row["CommentId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["CommentId"]),
                        OriginalFileName = row["OriginalFileName"].ToString(),
                        StoredFileName = row["StoredFileName"].ToString(),
                        CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    };
                }
            }
        }

        public List<TicketAttachment> GetByTicketId(int ticketId)
        {
            var list = new List<TicketAttachment>();
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAttachmentGetByTicketId"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new TicketAttachment
                        {
                            AttachmentId = Convert.ToInt32(row["AttachmentId"]),
                            TicketId = Convert.ToInt32(row["TicketId"]),
                            CommentId = row["CommentId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["CommentId"]),
                            OriginalFileName = row["OriginalFileName"].ToString(),
                            StoredFileName = row["StoredFileName"].ToString(),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                        });
                    }
                }
            }
            return list;
        }

        public void Delete(int attachmentId, int modifiedBy)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAttachmentDelete"))
            {
                db.AddInParameter(cmd, "@AttachmentId", DbType.Int32, attachmentId);
                db.AddInParameter(cmd, "@ModifiedBy", DbType.Int32, modifiedBy);
                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
