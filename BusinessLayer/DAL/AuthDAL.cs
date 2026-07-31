using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using TicketResolver.Models;

namespace TicketResolver.DAL
{
    public class AuthDAL
    {
        private readonly Database db = DatabaseFactory.CreateDatabase();

        public User GetUserByEmail(string email)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserGetByEmail"))
            {
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        return new User
                        {
                            UserId = Convert.ToInt32(row["UserId"]),
                            RoleId = Convert.ToInt32(row["RoleId"]),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Email = row["Email"].ToString(),
                            Mobile = row["Mobile"].ToString(),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        };
                    }
                    return null;
                }
            }
        }

        public User GetUserById(int userId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserGetById"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        return new User
                        {
                            UserId = Convert.ToInt32(row["UserId"]),
                            RoleId = Convert.ToInt32(row["RoleId"]),
                            FirstName = row["FirstName"].ToString(),
                            LastName = row["LastName"].ToString(),
                            Email = row["Email"].ToString(),
                            Mobile = row["Mobile"].ToString(),
                            CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        };
                    }
                    return null;
                }
            }
        }

        public string GetPasswordHashByUserId(int userId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserCredentialGetByUserId"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return ds.Tables[0].Rows[0]["PasswordHash"].ToString();
                    }
                    return null;
                }
            }
        }

        public int InsertUser(int roleId, string firstName, string lastName, string email, string mobile)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserInsert"))
            {
                db.AddInParameter(cmd, "@RoleId", DbType.Int32, roleId);
                db.AddInParameter(cmd, "@FirstName", DbType.String, firstName);
                db.AddInParameter(cmd, "@LastName", DbType.String, lastName);
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                db.AddInParameter(cmd, "@Mobile", DbType.String, mobile);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void InsertUserCredential(int userId, string passwordHash)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserCredentialInsert"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@PasswordHash", DbType.String, passwordHash);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void UpdatePassword(int userId, string passwordHash)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserCredentialUpdatePassword"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@PasswordHash", DbType.String, passwordHash);
                db.ExecuteNonQuery(cmd);
            }
        }

        public int InsertRefreshToken(int userId, string tokenHash, DateTime expiryDate)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenInsert"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@TokenHash", DbType.String, tokenHash);
                db.AddInParameter(cmd, "@ExpiryDate", DbType.DateTime, expiryDate);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public RefreshToken GetRefreshTokenByHash(string tokenHash)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenGetByTokenHash"))
            {
                db.AddInParameter(cmd, "@TokenHash", DbType.String, tokenHash);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        return new RefreshToken
                        {
                            RefreshTokenId = Convert.ToInt32(row["RefreshTokenId"]),
                            UserId = Convert.ToInt32(row["UserId"]),
                            TokenHash = row["TokenHash"].ToString(),
                            ExpiryDate = Convert.ToDateTime(row["ExpiryDate"]),
                            LastUsedDate = row["LastUsedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["LastUsedDate"]),
                            RevokedDate = row["RevokedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["RevokedDate"]),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        };
                    }
                    return null;
                }
            }
        }

        public void DeactivateAllRefreshTokens(int userId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenDeactivateAll"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void RevokeRefreshToken(int refreshTokenId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenRevoke"))
            {
                db.AddInParameter(cmd, "@RefreshTokenId", DbType.Int32, refreshTokenId);
                db.ExecuteNonQuery(cmd);
            }
        }

        public int RotateRefreshToken(int oldRefreshTokenId, string newTokenHash, DateTime newExpiryDate)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenRotate"))
            {
                db.AddInParameter(cmd, "@OldRefreshTokenId", DbType.Int32, oldRefreshTokenId);
                db.AddInParameter(cmd, "@NewTokenHash", DbType.String, newTokenHash);
                db.AddInParameter(cmd, "@NewExpiryDate", DbType.DateTime, newExpiryDate);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void UpdateRefreshTokenLastUsed(int refreshTokenId)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverRefreshTokenUpdateLastUsed"))
            {
                db.AddInParameter(cmd, "@RefreshTokenId", DbType.Int32, refreshTokenId);
                db.ExecuteNonQuery(cmd);
            }
        }

        public int InsertOtpVerification(int? userId, string email, string otpCode, string purpose, DateTime expiryDate)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverOtpInsert"))
            {
                if (userId.HasValue)
                    db.AddInParameter(cmd, "@UserId", DbType.Int32, userId.Value);
                else
                    db.AddInParameter(cmd, "@UserId", DbType.Int32, DBNull.Value);
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                db.AddInParameter(cmd, "@OtpCode", DbType.String, otpCode);
                db.AddInParameter(cmd, "@Purpose", DbType.String, purpose);
                db.AddInParameter(cmd, "@ExpiryDate", DbType.DateTime, expiryDate);
                return Convert.ToInt32(db.ExecuteScalar(cmd));
            }
        }

        public void InvalidatePreviousOtps(string email, string purpose)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverOtpInvalidatePrevious"))
            {
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                db.AddInParameter(cmd, "@Purpose", DbType.String, purpose);
                db.ExecuteNonQuery(cmd);
            }
        }

        public OtpVerifyResult VerifyOtp(string email, string otpCode, string purpose)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverOtpVerify"))
            {
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                db.AddInParameter(cmd, "@OtpCode", DbType.String, otpCode);
                db.AddInParameter(cmd, "@Purpose", DbType.String, purpose);
                using (DataSet ds = db.ExecuteDataSet(cmd))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        return new OtpVerifyResult
                        {
                            OtpId = row["OtpId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["OtpId"]),
                            UserId = row["UserId"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["UserId"]),
                            IsValid = Convert.ToBoolean(row["IsValid"])
                        };
                    }
                    return new OtpVerifyResult { IsValid = false };
                }
            }
        }
        public DataSet GetSupportExecutives()
        {
            return SearchUsers(null, 2, true, 1, 9999);
        }

        public DataSet SearchUsers(string searchTerm, int? roleId, bool? isActive, int pageNumber, int pageSize)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserSearch"))
            {
                db.AddInParameter(cmd, "@SearchTerm", DbType.String, searchTerm ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@RoleId", DbType.Int32, roleId ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@IsActive", DbType.Boolean, isActive ?? (object)DBNull.Value);
                db.AddInParameter(cmd, "@PageNumber", DbType.Int32, pageNumber);
                db.AddInParameter(cmd, "@PageSize", DbType.Int32, pageSize);
                return db.ExecuteDataSet(cmd);
            }
        }

        public void UpdateUser(int userId, int roleId, string firstName, string lastName, string email, string mobile)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserUpdate"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@RoleId", DbType.Int32, roleId);
                db.AddInParameter(cmd, "@FirstName", DbType.String, firstName);
                db.AddInParameter(cmd, "@LastName", DbType.String, lastName);
                db.AddInParameter(cmd, "@Email", DbType.String, email);
                db.AddInParameter(cmd, "@Mobile", DbType.String, mobile);
                db.ExecuteNonQuery(cmd);
            }
        }

        public void SetActiveStatus(int userId, bool isActive)
        {
            using (DbCommand cmd = db.GetStoredProcCommand("TicketResolverUserSetActiveStatus"))
            {
                db.AddInParameter(cmd, "@UserId", DbType.Int32, userId);
                db.AddInParameter(cmd, "@IsActive", DbType.Boolean, isActive);
                db.ExecuteNonQuery(cmd);
            }
        }
    }

    public class OtpVerifyResult
    {
        public int? OtpId { get; set; }
        public int? UserId { get; set; }
        public bool IsValid { get; set; }
    }
}
