using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Helpers;

namespace TicketResolver.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizeAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var identity = httpContext.User.Identity;
            if (!identity.IsAuthenticated)
                return false;

            if (_allowedRoles == null || _allowedRoles.Length == 0)
                return true;

            var roleClaim = ((ClaimsPrincipal)httpContext.User).FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
                return false;

            var roleName = roleClaim.Value;
            int roleId = roleName == "Administrator" ? 1 : roleName == "Support Executive" ? 2 : 3;

            return _allowedRoles.Contains(roleId);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Auth/Login");
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Auth/AccessDenied");
            }
        }
    }
}
