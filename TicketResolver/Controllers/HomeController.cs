using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Filters;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;

namespace TicketResolver.Controllers
{
    [RoleAuthorize]
    public class HomeController : Controller
    {
        private readonly DashboardDAL dashboardDAL = new DashboardDAL();
        private readonly TicketDAL ticketDAL = new TicketDAL();
        private readonly MasterDAL masterDAL = new MasterDAL();

        private int? CurrentUserId
        {
            get
            {
                var claim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier);
                return claim != null ? int.Parse(claim.Value) : (int?)null;
            }
        }

        private int CurrentRoleId
        {
            get
            {
                var claim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.Role);
                if (claim == null) return 0;
                switch (claim.Value)
                {
                    case "Administrator": return 1;
                    case "Support Executive": return 2;
                    default: return 3;
                }
            }
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            Response.StatusCode = 500;
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult Index()
        {
            try
            {
                var ds = dashboardDAL.GetStats(CurrentUserId, CurrentRoleId);
                var row = ds.Tables[0].Rows[0];

                var model = new DashboardViewModel
                {
                    TotalTickets = Convert.ToInt32(row["TotalTickets"]),
                    NewTickets = Convert.ToInt32(row["NewTickets"]),
                    AssignedTickets = Convert.ToInt32(row["AssignedTickets"]),
                    InProgressTickets = Convert.ToInt32(row["InProgressTickets"]),
                    ResolvedTickets = Convert.ToInt32(row["ResolvedTickets"]),
                    ClosedTickets = Convert.ToInt32(row["ClosedTickets"]),
                    ReopenedTickets = Convert.ToInt32(row["ReopenedTickets"]),
                    RecentTickets = GetRecentTickets()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("HomeController.Index", "Failed to load dashboard", ex);
                return View("Error");
            }
        }

        public JsonResult GetChartData(string groupBy)
        {
            try
            {
                int? createdBy = null;
                int? assignedTo = null;
                if (CurrentRoleId == 3)
                    createdBy = CurrentUserId;
                else if (CurrentRoleId == 2)
                    assignedTo = CurrentUserId;

                var model = new TicketSearchViewModel
                {
                    AssignedTo = assignedTo,
                    CreatedBy = createdBy,
                    PageNumber = 1,
                    PageSize = 500
                };
                var ds = ticketDAL.Search(model);
                var groups = groupBy == "Priority"
                    ? ds.Tables[0].Rows.Cast<DataRow>().GroupBy(r => r["PriorityName"].ToString())
                    : ds.Tables[0].Rows.Cast<DataRow>().GroupBy(r => r["StatusName"].ToString());

                var result = groups
                    .Select(g => new { label = g.Key, value = g.Count() })
                    .ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppLogger.Error("HomeController.GetChartData", "Failed to load chart data", ex);
                return Json(new { error = "Failed to load chart data." });
            }
        }

        private System.Collections.Generic.List<TicketListItemViewModel> GetRecentTickets()
        {
            try
            {
                int? createdBy = null;
                int? assignedTo = null;
                var role = CurrentRoleId;

                if (role == 3)
                    createdBy = CurrentUserId;
                else if (role == 2)
                    assignedTo = CurrentUserId;

                var model = new TicketSearchViewModel
                {
                    AssignedTo = assignedTo,
                    CreatedBy = createdBy,
                    PageNumber = 1,
                    PageSize = 5
                };
                var ds = ticketDAL.Search(model);
                return ds.Tables[0].Rows.Cast<DataRow>().Select(r => new TicketListItemViewModel
                {
                    TicketId = Convert.ToInt32(r["TicketId"]),
                    TicketNumber = r["TicketNumber"].ToString(),
                    Subject = r["Subject"].ToString(),
                    CategoryName = r["CategoryName"].ToString(),
                    PriorityName = r["PriorityName"].ToString(),
                    StatusName = r["StatusName"].ToString(),
                    CreatedByName = r["CreatedByName"].ToString(),
                    AssignedToName = r["AssignedToName"].ToString(),
                    CreatedDate = Convert.ToDateTime(r["CreatedDate"])
                }).ToList();
            }
            catch (Exception ex)
            {
                AppLogger.Error("HomeController.GetRecentTickets", "Failed to load recent tickets", ex);
                return new System.Collections.Generic.List<TicketListItemViewModel>();
            }
        }
    }
}
