using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class MasterDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public List<TicketCategory> GetCategories()
        {
            var list = new List<TicketCategory>();
            using (DataSet ds = db.ExecuteDataSet(db.GetStoredProcCommand("TicketResolverTicketCategoryGetAll")))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new TicketCategory
                    {
                        CategoryId = Convert.ToInt32(row["CategoryId"]),
                        CategoryName = row["CategoryName"].ToString()
                    });
                }
            }
            return list;
        }

        public List<TicketPriority> GetPriorities()
        {
            var list = new List<TicketPriority>();
            using (DataSet ds = db.ExecuteDataSet(db.GetStoredProcCommand("TicketResolverTicketPriorityGetAll")))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new TicketPriority
                    {
                        PriorityId = Convert.ToInt32(row["PriorityId"]),
                        PriorityName = row["PriorityName"].ToString(),
                        Sequence = Convert.ToInt32(row["Sequence"])
                    });
                }
            }
            return list;
        }

        public List<TicketStatus> GetStatuses()
        {
            var list = new List<TicketStatus>();
            using (DataSet ds = db.ExecuteDataSet(db.GetStoredProcCommand("TicketResolverTicketStatusGetAll")))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new TicketStatus
                    {
                        StatusId = Convert.ToInt32(row["StatusId"]),
                        StatusName = row["StatusName"].ToString(),
                        IsTerminalState = Convert.ToBoolean(row["IsTerminalState"])
                    });
                }
            }
            return list;
        }

        public List<TicketRole> GetRoles()
        {
            var list = new List<TicketRole>();
            using (DataSet ds = db.ExecuteDataSet(db.GetStoredProcCommand("TicketResolverRoleGetAll")))
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new TicketRole
                    {
                        RoleId = Convert.ToInt32(row["RoleId"]),
                        RoleName = row["RoleName"].ToString()
                    });
                }
            }
            return list;
        }

        // Category CRUD
        public int InsertCategory(string categoryName)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketCategoryInsert"))
            {
                db.AddInParameter(cmd, "@CategoryName", DbType.String, categoryName);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void UpdateCategory(int categoryId, string categoryName)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketCategoryUpdate"))
            {
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, categoryId);
                db.AddInParameter(cmd, "@CategoryName", DbType.String, categoryName);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketCategoryDelete"))
            {
                db.AddInParameter(cmd, "@CategoryId", DbType.Int32, categoryId);
                db.ExecuteNonQuery(cmd);
            }
        }

        // Priority CRUD
        public int InsertPriority(string priorityName, int sequence)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketPriorityInsert"))
            {
                db.AddInParameter(cmd, "@PriorityName", DbType.String, priorityName);
                db.AddInParameter(cmd, "@Sequence", DbType.Int32, sequence);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void UpdatePriority(int priorityId, string priorityName, int sequence)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketPriorityUpdate"))
            {
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, priorityId);
                db.AddInParameter(cmd, "@PriorityName", DbType.String, priorityName);
                db.AddInParameter(cmd, "@Sequence", DbType.Int32, sequence);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void DeletePriority(int priorityId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketPriorityDelete"))
            {
                db.AddInParameter(cmd, "@PriorityId", DbType.Int32, priorityId);
                db.ExecuteNonQuery(cmd);
            }
        }

        // Status CRUD
        public int InsertStatus(string statusName, bool isTerminalState)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketStatusInsert"))
            {
                db.AddInParameter(cmd, "@StatusName", DbType.String, statusName);
                db.AddInParameter(cmd, "@IsTerminalState", DbType.Boolean, isTerminalState);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void UpdateStatus(int statusId, string statusName, bool isTerminalState)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketStatusUpdate"))
            {
                db.AddInParameter(cmd, "@StatusId", DbType.Int32, statusId);
                db.AddInParameter(cmd, "@StatusName", DbType.String, statusName);
                db.AddInParameter(cmd, "@IsTerminalState", DbType.Boolean, isTerminalState);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void DeleteStatus(int statusId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverTicketStatusDelete"))
            {
                db.AddInParameter(cmd, "@StatusId", DbType.Int32, statusId);
                db.ExecuteNonQuery(cmd);
            }
        }
    }
}
