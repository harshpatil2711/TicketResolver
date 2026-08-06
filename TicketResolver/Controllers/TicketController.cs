using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Filters;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;


namespace TicketResolver.Controllers
{
    [RoleAuthorize]
    public class TicketController : Controller
    {
        private readonly TicketDAL ticketDAL = new TicketDAL();
        private readonly MasterDAL masterDAL = new MasterDAL();
        private readonly CommentDAL commentDAL = new CommentDAL();
        private readonly AttachmentDAL attachmentDAL = new AttachmentDAL();
        private readonly HistoryDAL historyDAL = new HistoryDAL();
        private readonly AuthDAL authDAL = new AuthDAL();

        private int CurrentUserId => int.Parse(((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value);
        private string CurrentRole => ((ClaimsPrincipal)User).FindFirst(ClaimTypes.Role).Value;

        [HttpGet]
        public ActionResult Index(string searchTerm, int? categoryId, int? priorityId, int? statusId, int page = 1, int size = 10, string sortColumn = "Created", string sortDirection = "DESC", bool? isUnassigned = null)
        {
            try
            {
                var model = BuildTicketSearchModel(searchTerm, categoryId, priorityId, statusId, page, size, sortColumn, sortDirection, isUnassigned);
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Index", "Failed to load ticket list", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TicketSearchViewModel input)
        {
            try
            {
                var model = BuildTicketSearchModel(input.SearchTerm, input.CategoryId, input.PriorityId, input.StatusId, input.PageNumber, input.PageSize, input.SortColumn, input.SortDirection, input.IsUnassigned);
                return PartialView("_TicketTable", model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Index", "Failed to load ticket list (AJAX)", ex);
                return Content("<div class='alert alert-danger m-3'>Error loading tickets.</div>");
            }
        }

        private TicketSearchViewModel BuildTicketSearchModel(string searchTerm, int? categoryId, int? priorityId, int? statusId, int page, int size = 10, string sortColumn = "Created", string sortDirection = "DESC", bool? isUnassigned = null)
        {
            int? createdBy = null;
            int? assignedTo = null;
            var isAdmin = CurrentRole == "Administrator";
            if (CurrentRole == "Employee")
                createdBy = CurrentUserId;
            else if (CurrentRole == "Support Executive")
                assignedTo = CurrentUserId;

            var model = new TicketSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                PriorityId = priorityId,
                StatusId = statusId,
                AssignedTo = assignedTo,
                CreatedBy = createdBy,
                PageNumber = page,
                PageSize = size,
                SortColumn = sortColumn,
                SortDirection = sortDirection,
                IsUnassigned = isAdmin ? isUnassigned : null
            };

            var ds = ticketDAL.Search(model);

            model.TotalCount = ds.Tables[0].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]) : 0;
            model.Results = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new TicketListItemViewModel
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
            model.Categories = masterDAL.GetCategories();
            model.Priorities = masterDAL.GetPriorities();
            model.Statuses = masterDAL.GetStatuses();

            return model;
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = new TicketCreateViewModel
                {
                    Categories = masterDAL.GetCategories(),
                    Priorities = masterDAL.GetPriorities()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Create", "Failed to load create form", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketCreateViewModel model, HttpPostedFileBase[] files)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }

            try
            {
                var ticketNumber = ticketDAL.GenerateTicketNumber();
                model.TicketNumber = ticketNumber;
                model.CreatedBy = CurrentUserId;
                var ticketId = ticketDAL.Insert(model);

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file == null || file.ContentLength == 0) continue;
                        SaveAttachment(ticketId, null, file);
                    }
                }

                TempData["SuccessMessage"] = $"Ticket {ticketNumber} created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Create", "Failed to create ticket", ex);
                ModelState.AddModelError("", "An error occurred while creating the ticket.");
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            try
            {
                var ds = ticketDAL.GetDetailById(id);
                if (ds.Tables[0].Rows.Count == 0)
                    return HttpNotFound();

                var row = ds.Tables[0].Rows[0];
                var model = new TicketDetailViewModel
                {
                    TicketId = Convert.ToInt32(row["TicketId"]),
                    TicketNumber = row["TicketNumber"].ToString(),
                    Subject = row["Subject"].ToString(),
                    Description = row["Description"].ToString(),
                    StatusId = Convert.ToInt32(row["StatusId"]),
                    CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ResolvedDate = row["ResolvedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ResolvedDate"]),
                    CategoryName = row["CategoryName"].ToString(),
                    PriorityName = row["PriorityName"].ToString(),
                    StatusName = row["StatusName"].ToString(),
                    IsTerminalState = Convert.ToBoolean(row["IsTerminalState"]),
                    CreatedByName = row["CreatedByName"].ToString(),
                    AssignedToName = row["AssignedToName"]?.ToString(),
                    AssignedTo = row["AssignedTo"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["AssignedTo"]),
                    Comments = commentDAL.GetByTicketId(Convert.ToInt32(row["TicketId"]), CurrentUserId, GetRoleId()),
                    Attachments = attachmentDAL.GetByTicketId(Convert.ToInt32(row["TicketId"])),
                    History = historyDAL.GetByTicketId(Convert.ToInt32(row["TicketId"])),
                    Statuses = masterDAL.GetStatuses()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Details", $"Failed to load ticket details for id={id}", ex);
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var ticket = ticketDAL.GetById(id);
                if (ticket == null)
                    return HttpNotFound();

                if (ticket.CreatedBy != CurrentUserId && CurrentRole != "Administrator")
                    return RedirectToAction("AccessDenied", "Auth");

                if (CurrentRole == "Employee" && (ticket.StatusId != 1 || ticket.AssignedTo.HasValue))
                {
                    TempData["ErrorMessage"] = "You can only edit a ticket that is still New and unassigned.";
                    return RedirectToAction("Details", new { id });
                }

                var model = new TicketEditViewModel
                {
                    TicketId = ticket.TicketId,
                    Subject = ticket.Subject,
                    Description = ticket.Description,
                    CategoryId = ticket.CategoryId,
                    PriorityId = ticket.PriorityId,
                    Categories = masterDAL.GetCategories(),
                    Priorities = masterDAL.GetPriorities()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Edit", $"Failed to load edit form for id={id}", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }

            try
            {
                var ticket = ticketDAL.GetById(model.TicketId);
                if (ticket == null)
                    return HttpNotFound();

                if (ticket.CreatedBy != CurrentUserId && CurrentRole != "Administrator")
                    return RedirectToAction("AccessDenied", "Auth");

                if (CurrentRole == "Employee" && (ticket.StatusId != 1 || ticket.AssignedTo.HasValue))
                {
                    ModelState.AddModelError("", "You can only edit a ticket that is still New and unassigned.");
                    model.Categories = masterDAL.GetCategories();
                    model.Priorities = masterDAL.GetPriorities();
                    return View(model);
                }

                model.ModifiedBy = CurrentUserId;
                var affected = ticketDAL.Update(model);
                if (affected == 0)
                {
                    TempData["ErrorMessage"] = "Cannot edit this ticket — it must be in New status and, for employees, unassigned.";
                    return RedirectToAction("Details", new { id = model.TicketId });
                }
                TempData["SuccessMessage"] = "Ticket updated successfully.";
                return RedirectToAction("Details", new { id = model.TicketId });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Edit", $"Failed to update ticket id={model.TicketId}", ex);
                ModelState.AddModelError("", "An error occurred while updating the ticket.");
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(1)]
        public ActionResult Delete(int id)
        {
            try
            {
                ticketDAL.Delete(id, CurrentUserId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Delete", $"Failed to delete ticket id={id}", ex);
                return Json(new { success = false, error = "Could not delete ticket." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int ticketId, string commentText, bool isInternalNote, HttpPostedFileBase file)
        {
            try
            {
                var commentId = commentDAL.Insert(ticketId, CurrentUserId, commentText, isInternalNote);

                if (file != null && file.ContentLength > 0)
                {
                    SaveAttachment(ticketId, commentId, file);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.AddComment", "Failed to add comment", ex);
                return Json(new { success = false, error = "Could not add comment." });
            }
        }

        public ActionResult Download(int id)
        {
            try
            {
                var attachment = attachmentDAL.GetById(id);
                if (attachment == null) return HttpNotFound();

                var path = Server.MapPath("~/Uploads/" + attachment.StoredFileName);
                if (!System.IO.File.Exists(path)) return HttpNotFound();

                return File(path, "application/octet-stream", attachment.OriginalFileName);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Download", $"Failed to download attachment id={id}", ex);
                return HttpNotFound();
            }
        }

        [HttpGet]
        [RoleAuthorize(1, 2)]
        public ActionResult Assign(int id)
        {
            try
            {
                var ticket = ticketDAL.GetById(id);
                if (ticket == null)
                    return HttpNotFound();

                var model = new TicketAssignViewModel
                {
                    TicketId = ticket.TicketId,
                    TicketNumber = ticket.TicketNumber,
                    Subject = ticket.Subject,
                    SupportExecutives = GetSupportExecutives()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Assign", $"Failed to load assign form for id={id}", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize(1, 2)]
        public ActionResult Assign(TicketAssignViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SupportExecutives = GetSupportExecutives();
                return View(model);
            }

            try
            {
                model.AssignedBy = CurrentUserId;
                ticketDAL.Assign(model);

                var ticket = ticketDAL.GetById(model.TicketId);
                var assignee = authDAL.GetUserById(model.AssignedTo);
                var assignedBy = authDAL.GetUserById(CurrentUserId);
                var template = EmailTemplates.LoadTemplate(Server.MapPath("~/EmailTemplates/NotificationEmail.html"));

                if (assignee != null)
                {
                    var body = EmailTemplates.PopulateNotificationTemplate(template, assignee.FirstName, ticket.TicketNumber,
                        "A ticket has been assigned to you.", assignedBy?.FirstName ?? "System");
                    EmailHelper.SendEmail(assignee.Email, $"Ticket {ticket.TicketNumber} - Assigned to you", body);
                }

                TempData["SuccessMessage"] = "Ticket assigned successfully.";
                return RedirectToAction("Details", new { id = model.TicketId });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Assign", $"Failed to assign ticket id={model.TicketId}", ex);
                ModelState.AddModelError("", "An error occurred while assigning the ticket.");
                model.SupportExecutives = GetSupportExecutives();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeStatus(int ticketId, int newStatusId, string changeReason)
        {
            try
            {
                ticketDAL.UpdateStatus(ticketId, newStatusId, CurrentUserId, changeReason);
                NotifyStatusChange(ticketId, newStatusId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.ChangeStatus", $"Failed to change status for ticket id={ticketId}", ex);
                return Json(new { success = false, error = "Could not change status." });
            }
        }

        private void NotifyStatusChange(int ticketId, int newStatusId)
        {
            try
            {
                var ticket = ticketDAL.GetById(ticketId);
                var creator = authDAL.GetUserById(ticket.CreatedBy);
                var actor = authDAL.GetUserById(CurrentUserId);
                var statuses = masterDAL.GetStatuses();
                var newStatusName = statuses.FirstOrDefault(s => s.StatusId == newStatusId)?.StatusName ?? "Unknown";
                var template = EmailTemplates.LoadTemplate(Server.MapPath("~/EmailTemplates/NotificationEmail.html"));

                var recipients = new List<KeyValuePair<string, string>>();
                if (creator != null)
                    recipients.Add(new KeyValuePair<string, string>(creator.Email, creator.FirstName));
                if (ticket.AssignedTo.HasValue && ticket.AssignedTo != ticket.CreatedBy)
                {
                    var assignee = authDAL.GetUserById(ticket.AssignedTo.Value);
                    if (assignee != null)
                        recipients.Add(new KeyValuePair<string, string>(assignee.Email, assignee.FirstName));
                }
                if (recipients.Count == 0)
                    return;

                var ticketNumber = ticket.TicketNumber;
                var subject = $"Ticket {ticketNumber} - Status: {newStatusName}";
                var message = $"Ticket status changed to {newStatusName}.";
                var actorName = actor?.FirstName ?? "System";

                HostingEnvironment.QueueBackgroundWorkItem(ct =>
                {
                    try
                    {
                        foreach (var recipient in recipients)
                        {
                            var body = EmailTemplates.PopulateNotificationTemplate(template, recipient.Value, ticketNumber, message, actorName);
                            EmailHelper.SendEmail(recipient.Key, subject, body);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Error("TicketController.NotifyStatusChange", $"Failed to send status-change email for ticket id={ticketId}", ex);
                    }
                });
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.NotifyStatusChange", $"Failed to prepare status-change notification for ticket id={ticketId}", ex);
            }
        }

        public ActionResult Export(string searchTerm, int? categoryId, int? priorityId, int? statusId, bool? isUnassigned)
        {
            try
            {
                var isAdmin = CurrentRole == "Administrator";
                int? createdBy = null;
                int? assignedTo = null;
                if (CurrentRole == "Employee")
                    createdBy = CurrentUserId;
                else if (CurrentRole == "Support Executive")
                    assignedTo = CurrentUserId;

                var model = new TicketSearchViewModel
                {
                    SearchTerm = searchTerm,
                    CategoryId = categoryId,
                    PriorityId = priorityId,
                    StatusId = statusId,
                    AssignedTo = assignedTo,
                    CreatedBy = createdBy,
                    PageNumber = 1,
                    PageSize = 9999,
                    SortColumn = "CreatedDate",
                    SortDirection = "DESC",
                    IsUnassigned = isAdmin ? isUnassigned : null
                };
                var ds = ticketDAL.Search(model);
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Ticket#,Subject,Category,Priority,Status,CreatedBy,AssignedTo,CreatedDate");

                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    csv.AppendLine(string.Join(",",
                        r["TicketNumber"],
                        "\"" + r["Subject"].ToString().Replace("\"", "\"\"") + "\"",
                        r["CategoryName"],
                        r["PriorityName"],
                        r["StatusName"],
                        r["CreatedByName"],
                        r["AssignedToName"],
                        Convert.ToDateTime(r["CreatedDate"]).ToString("yyyy-MM-dd")));
                }

                return File(new System.Text.UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "Tickets.csv");
            }
            catch (Exception ex)
            {
                AppLogger.Error("TicketController.Export", "Failed to export tickets", ex);
                return RedirectToAction("Index");
            }
        }

        private int GetRoleId()
        {
            switch (CurrentRole)
            {
                case "Administrator": return 1;
                case "Support Executive": return 2;
                default: return 3;
            }
        }

        private void SaveAttachment(int ticketId, int? commentId, HttpPostedFileBase file)
        {
            var uploadDir = Server.MapPath("~/Uploads");
            var storedName = AttachmentHelper.SaveFile(file, uploadDir);
            attachmentDAL.Insert(ticketId, commentId, Path.GetFileName(file.FileName), storedName, CurrentUserId);
        }

        private System.Collections.Generic.List<UserListItem> GetSupportExecutives()
        {
            var ds = authDAL.GetSupportExecutives();
            return ds.Tables[0].Rows.Cast<DataRow>().Select(r => new UserListItem
            {
                UserId = Convert.ToInt32(r["UserId"]),
                FullName = r["FirstName"].ToString() + " " + r["LastName"].ToString(),
                Email = r["Email"].ToString()
            }).ToList();
        }
    }
}
