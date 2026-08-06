using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;
using TicketResolver.ViewModels;

namespace TicketResolver.DAL
{
    public class TicketDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public string GenerateTicketNumber()
        {
            return db.ExecuteDataSet(db.GetStoredProcCommand("TicketResolverTicketGenerateNumber"))
                       .Tables[0].Rows[0]["TicketNumber"].ToString();
        }

        public int Insert(TicketCreateViewModel model)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketInsert"))
            {
                db.AddInParameter(cmd, "@TicketNumber", DbType.String, model.TicketNumber);
                db.AddInParameter(cmd, "@Subject", DbType.String, model.Subject);
                db.AddInParameter(cmd, "@Description", DbType.String, model.Description);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, model.CategoryId);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, model.PriorityId);
                db.AddInParameter(cmd, "@CreatedBy", DbType.Int32, model.CreatedBy);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public int Update(TicketEditViewModel model)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketUpdate"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, model.TicketId);
                db.AddInParameter(cmd, "@Subject", DbType.String, model.Subject);
                db.AddInParameter(cmd, "@Description", DbType.String, model.Description);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, model.CategoryId);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, model.PriorityId);
                db.AddInParameter(cmd, "@ModifiedBy", DbType.Int32, model.ModifiedBy);
                db.AddOutParameter(cmd, "@AffectedRows", DbType.Int32, 4);
                db.ExecuteNonQuery(cmd);
                return Convert.ToInt32(cmd.Parameters["@AffectedRows"].Value);
            }
        }

        public Ticket GetById(int ticketId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketGetById"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count == 0) return null;
                    DataRow row = ds.Tables[0].Rows[0];
                    return new Ticket
                    {
                        TicketId = Convert.ToInt32(row["TicketId"]),
                        TicketNumber = row["TicketNumber"].ToString(),
                        Subject = row["Subject"].ToString(),
                        Description = row["Description"].ToString(),
                        CategoryId = Convert.ToInt32(row["CategoryId"]),
                        PriorityId = Convert.ToInt32(row["PriorityId"]),
                        StatusId = Convert.ToInt32(row["StatusId"]),
                        CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                        AssignedTo = row["AssignedTo"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["AssignedTo"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        ResolvedDate = row["ResolvedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ResolvedDate"]),
                        ClosedDate = row["ClosedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ClosedDate"]),
                        ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["ModifiedBy"]),
                        ModifiedDate = row["ModifiedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ModifiedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                }
            }
        }

        public DataSet GetDetailById(int ticketId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketGetById"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                return db.ExecuteDataSet(cmd);
            }
        }

        public DataSet Search(TicketSearchViewModel model)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketSearch"))
            {
                db.AddInParameter(cmd, "@SearchTerm", DbType.String, model.SearchTerm ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, model.CategoryId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, model.PriorityId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@StatusId", DbType.Int32, model.StatusId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@AssignedTo", DbType.Int32, model.AssignedTo ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@CreatedBy", DbType.Int32, model.CreatedBy ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@PageNumber", DbType.Int32, model.PageNumber);
                db.AddInParameter(cmd, "@PageSize", DbType.Int32, model.PageSize);
                db.AddInParameter(cmd, "@SortColumn", DbType.String, model.SortColumn);
                db.AddInParameter(cmd, "@SortDirection", DbType.String, model.SortDirection);
                db.AddInParameter(cmd, "@IsUnassigned", DbType.Boolean, model.IsUnassigned ?? (object)DBNull.Value);
                return db.ExecuteDataSet(cmd);
            }
        }

        public void UpdateStatus(int ticketId, int newStatusId, int modifiedBy, string changeReason)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketUpdateStatus"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@NewStatusId", DbType.Int32, newStatusId);
                db.AddInParameter(cmd, "@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter(cmd, "@ChangeReason", DbType.String, changeReason ?? (object)DBNull.Value);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void Assign(TicketAssignViewModel model)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAssign"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, model.TicketId);
                db.AddInParameter(cmd, "@AssignedTo", DbType.Int32, model.AssignedTo);
                db.AddInParameter(cmd, "@AssignedBy", DbType.Int32, model.AssignedBy);
                db.AddInParameter(cmd, "@ChangeReason", DbType.String, model.ChangeReason ?? (object)DBNull.Value);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void Delete(int ticketId, int modifiedBy)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketDelete"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@ModifiedBy", DbType.Int32, modifiedBy);
                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
