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
                    RecentTickets = GetRecentTickets(),
                    UnassignedTickets = GetUnassignedTickets(),
                    Priorities = masterDAL.GetPriorities(),
                    Statuses = masterDAL.GetStatuses()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("HomeController.Index", "Failed to load dashboard", ex);
                return View("Error");
            }
        }

        public JsonResult GetChartData(int? priorityId, int? statusId)
        {
            try
            {
                int? createdBy = null;
                int? assignedTo = null;
                if (CurrentRoleId == 3)
                    createdBy = CurrentUserId;
                else if (CurrentRoleId == 2)
                    assignedTo = CurrentUserId;

                var ds = ticketDAL.Search(null, null, priorityId, statusId, assignedTo, createdBy, 1, 500);
                var groups = statusId.HasValue
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

        private System.Collections.Generic.List<TicketListItemViewModel> GetUnassignedTickets()
        {
            if (CurrentRoleId == 3) return new System.Collections.Generic.List<TicketListItemViewModel>();

            var ds = ticketDAL.Search(null, null, null, 1, null, null, 1, 5);
            return ds.Tables[0].Rows.Cast<DataRow>().Select(r => new TicketListItemViewModel
            {
                TicketId = Convert.ToInt32(r["TicketId"]),
                TicketNumber = r["TicketNumber"].ToString(),
                Subject = r["Subject"].ToString(),
                CategoryName = r["CategoryName"].ToString(),
                PriorityName = r["PriorityName"].ToString(),
                StatusName = r["StatusName"].ToString(),
                CreatedByName = r["CreatedByName"].ToString(),
                CreatedDate = Convert.ToDateTime(r["CreatedDate"])
            }).ToList();
        }

        private System.Collections.Generic.List<TicketListItemViewModel> GetRecentTickets()
        {
            int? createdBy = null;
            int? assignedTo = null;
            var role = CurrentRoleId;

            if (role == 3)
                createdBy = CurrentUserId;
            else if (role == 2)
                assignedTo = CurrentUserId;

            var ds = ticketDAL.Search(null, null, null, null, assignedTo, createdBy, 1, 5);
            return ds.Tables[0].Rows.Cast<DataRow>().Select(r => new TicketListItemViewModel
            {
                TicketId = Convert.ToInt32(r["TicketId"]),
                TicketNumber = r["TicketNumber"].ToString(),
                Subject = r["Subject"].ToString(),
                CategoryName = r["CategoryName"].ToString(),
                PriorityName = r["PriorityName"].ToString(),
                PrioritySequence = Convert.ToInt32(r["PrioritySequence"]),
                StatusName = r["StatusName"].ToString(),
                StatusId = Convert.ToInt32(r["StatusIdVal"]),
                CreatedByName = r["CreatedByName"].ToString(),
                AssignedToName = r["AssignedToName"].ToString(),
                CreatedDate = Convert.ToDateTime(r["CreatedDate"])
            }).ToList();
        }
    }
}
