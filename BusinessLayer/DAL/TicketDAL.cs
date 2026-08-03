using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

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

        public int Insert(string ticketNumber, string subject, string description, int categoryId, int priorityId, int createdBy)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketInsert"))
            {
                db.AddInParameter(cmd, "@TicketNumber", DbType.String, ticketNumber);
                db.AddInParameter(cmd, "@Subject", DbType.String, subject);
                db.AddInParameter(cmd, "@Description", DbType.String, description);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, categoryId);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, priorityId);
                db.AddInParameter(cmd, "@CreatedBy", DbType.Int32, createdBy);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void Update(int ticketId, string subject, string description, int categoryId, int priorityId, int modifiedBy)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketUpdate"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@Subject", DbType.String, subject);
                db.AddInParameter(cmd, "@Description", DbType.String, description);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, categoryId);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, priorityId);
                db.AddInParameter(cmd, "@ModifiedBy", DbType.Int32, modifiedBy);
                db.ExecuteNonQuery(cmd);
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

        public DataSet Search(string searchTerm, int? categoryId, int? priorityId, int? statusId, int? assignedTo, int? createdBy, int pageNumber, int pageSize, string sortColumn = "CreatedDate", string sortDirection = "DESC", bool? isUnassigned = null)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketSearch"))
            {
                db.AddInParameter(cmd, "@SearchTerm", DbType.String, searchTerm ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, categoryId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, priorityId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@StatusId", DbType.Int32, statusId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@AssignedTo", DbType.Int32, assignedTo ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@CreatedBy", DbType.Int32, createdBy ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@PageNumber", DbType.Int32, pageNumber);
                db.AddInParameter(cmd, "@PageSize", DbType.Int32, pageSize);
                db.AddInParameter(cmd, "@SortColumn", DbType.String, sortColumn);
                db.AddInParameter(cmd, "@SortDirection", DbType.String, sortDirection);
                db.AddInParameter(cmd, "@IsUnassigned", DbType.Boolean, isUnassigned ?? (object)DBNull.Value);
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

        public void Assign(int ticketId, int assignedTo, int assignedBy, string changeReason)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketAssign"))
            {
                db.AddInParameter(cmd, "@TicketId", DbType.Int32, ticketId);
                db.AddInParameter(cmd, "@AssignedTo", DbType.Int32, assignedTo);
                db.AddInParameter(cmd, "@AssignedBy", DbType.Int32, assignedBy);
                db.AddInParameter(cmd, "@ChangeReason", DbType.String, changeReason ?? (object)DBNull.Value);
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
