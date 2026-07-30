using System;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;
using TicketResolver.Helpers;

namespace TicketResolver
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Server.MapPath("~/logs/log-.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.Information("Application started");
        }

        protected void Application_PostAuthenticateRequest()
        {
            var cookie = Request.Cookies["jwt_token"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                try
                {
                    var principal = JwtHelper.ValidateToken(cookie.Value);
                    if (principal != null)
                    {
                        HttpContext.Current.User = principal;
                        Thread.CurrentPrincipal = principal;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Warning("Global.PostAuthenticateRequest", "JWT validation failed, attempting refresh token");
                    var refreshCookie = Request.Cookies["refresh_token"];
                    if (refreshCookie != null && !string.IsNullOrEmpty(refreshCookie.Value))
                    {
                        TryRefreshToken(refreshCookie.Value);
                    }
                }
            }
        }

        private void TryRefreshToken(string refreshTokenValue)
        {
            try
            {
                var dal = new DAL.AuthDAL();
                var tokenHash = JwtHelper.HashRefreshToken(refreshTokenValue);
                var storedToken = dal.GetRefreshTokenByHash(tokenHash);

                if (storedToken == null || !storedToken.IsActive || storedToken.ExpiryDate <= DateTime.UtcNow)
                    return;

                dal.UpdateRefreshTokenLastUsed(storedToken.RefreshTokenId);

                var user = dal.GetUserById(storedToken.UserId);
                if (user == null || !user.IsActive)
                    return;

                var roleName = user.RoleId == 1 ? "Administrator" : user.RoleId == 2 ? "Support Executive" : "Employee";

                var newAccessToken = JwtHelper.GenerateAccessToken(user.UserId, user.Email, roleName);
                var newRefreshToken = JwtHelper.GenerateRefreshToken();
                var newRefreshTokenHash = JwtHelper.HashRefreshToken(newRefreshToken);
                var expiryDate = DateTime.UtcNow.AddDays(JwtHelper.GetRefreshTokenExpiryDays());

                dal.RotateRefreshToken(storedToken.RefreshTokenId, newRefreshTokenHash, expiryDate);

                var accessCookie = new HttpCookie("jwt_token", newAccessToken) { HttpOnly = true, Secure = false, Expires = expiryDate };
                var refreshCookie = new HttpCookie("refresh_token", newRefreshToken) { HttpOnly = true, Secure = false, Expires = expiryDate };
                Response.Cookies.Add(accessCookie);
                Response.Cookies.Add(refreshCookie);

                var principal = JwtHelper.ValidateToken(newAccessToken);
                if (principal != null)
                {
                    HttpContext.Current.User = principal;
                    Thread.CurrentPrincipal = principal;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Global.TryRefreshToken", "Failed to refresh token", ex);
            }
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            if (ex != null)
            {
                Log.Error(ex, "Unhandled exception");
            }
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}
