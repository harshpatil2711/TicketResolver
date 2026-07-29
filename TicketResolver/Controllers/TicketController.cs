using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
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
        public ActionResult Index(string searchTerm, int? categoryId, int? priorityId, int? statusId, int page = 1)
        {
            int? createdBy = null;
            int? assignedTo = null;
            if (CurrentRole == "Employee")
                createdBy = CurrentUserId;
            else if (CurrentRole == "Support Executive")
                assignedTo = CurrentUserId;

            var ds = ticketDAL.Search(searchTerm, categoryId, priorityId, statusId, assignedTo, createdBy, page, 10);

            var model = new TicketSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                PriorityId = priorityId,
                StatusId = statusId,
                PageNumber = page,
                TotalCount = ds.Tables[1].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]) : 0,
                Results = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new TicketListItemViewModel
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
                    CreatedDate = Convert.ToDateTime(r["CreatedDate"]),
                    ResolvedDate = r["ResolvedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ResolvedDate"]),
                    ClosedDate = r["ClosedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(r["ClosedDate"])
                }).ToList(),
                Categories = masterDAL.GetCategories(),
                Priorities = masterDAL.GetPriorities(),
                Statuses = masterDAL.GetStatuses()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new TicketCreateViewModel
            {
                Categories = masterDAL.GetCategories(),
                Priorities = masterDAL.GetPriorities()
            };
            return View(model);
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
                var ticketId = ticketDAL.Insert(ticketNumber, model.Subject, model.Description, model.CategoryId, model.PriorityId, CurrentUserId);

                if (files != null)
                {
                    var uploadDir = Server.MapPath("~/Uploads");
                    foreach (var file in files)
                    {
                        if (file == null || file.ContentLength == 0) continue;
                        var storedName = AttachmentHelper.SaveFile(file, uploadDir);
                        attachmentDAL.Insert(ticketId, null, Path.GetFileName(file.FileName), storedName, CurrentUserId);
                    }
                }

                TempData["SuccessMessage"] = $"Ticket {ticketNumber} created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the ticket.");
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var ticket = ticketDAL.GetById(id);
            if (ticket == null)
                return HttpNotFound();

            var ds = ticketDAL.GetByCreatedBy(ticket.CreatedBy, 1, 1);

            var model = new TicketDetailViewModel
            {
                TicketId = ticket.TicketId,
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                PriorityId = ticket.PriorityId,
                StatusId = ticket.StatusId,
                CreatedBy = ticket.CreatedBy,
                CreatedDate = ticket.CreatedDate,
                ResolvedDate = ticket.ResolvedDate,
                ClosedDate = ticket.ClosedDate,
                IsActive = ticket.IsActive,
                Comments = commentDAL.GetByTicketId(ticket.TicketId, CurrentUserId, GetRoleId()),
                Attachments = attachmentDAL.GetByTicketId(ticket.TicketId),
                History = historyDAL.GetByTicketId(ticket.TicketId),
                Statuses = masterDAL.GetStatuses(),
                SupportExecutives = GetSupportExecutives()
            };

            if (ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                model.CategoryName = row["CategoryName"].ToString();
                model.PriorityName = row["PriorityName"].ToString();
                model.StatusName = row["StatusName"].ToString();
                model.IsTerminalState = Convert.ToBoolean(row["IsTerminalState"]);
                model.CreatedByName = row["CreatedByName"].ToString();
                model.AssignedToName = row["AssignedToName"]?.ToString();
                model.AssignedTo = row["AssignedTo"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["AssignedTo"]);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var ticket = ticketDAL.GetById(id);
            if (ticket == null)
                return HttpNotFound();

            if (ticket.CreatedBy != CurrentUserId && CurrentRole != "Administrator")
                return RedirectToAction("AccessDenied", "Auth");

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
                ticketDAL.Update(model.TicketId, model.Subject, model.Description, model.CategoryId, model.PriorityId, CurrentUserId);
                TempData["SuccessMessage"] = "Ticket updated successfully.";
                return RedirectToAction("Details", new { id = model.TicketId });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the ticket.");
                model.Categories = masterDAL.GetCategories();
                model.Priorities = masterDAL.GetPriorities();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                ticketDAL.Delete(id, CurrentUserId);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, error = "Could not delete ticket." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int ticketId, string commentText, bool isInternalNote)
        {
            try
            {
                commentDAL.Insert(ticketId, CurrentUserId, commentText, isInternalNote);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, error = "Could not add comment." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAttachment(int ticketId, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { success = false, error = "No file selected." });

            try
            {
                var uploadDir = Server.MapPath("~/Uploads");
                var storedName = AttachmentHelper.SaveFile(file, uploadDir);
                attachmentDAL.Insert(ticketId, null, Path.GetFileName(file.FileName), storedName, CurrentUserId);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, error = "Could not upload file." });
            }
        }

        public ActionResult Download(int id)
        {
            var attachment = attachmentDAL.GetById(id);
            if (attachment == null) return HttpNotFound();

            var path = Server.MapPath("~/Uploads/" + attachment.StoredFileName);
            if (!System.IO.File.Exists(path)) return HttpNotFound();

            return File(path, "application/octet-stream", attachment.OriginalFileName);
        }

        [HttpGet]
        public ActionResult Assign(int id)
        {
            var ticket = ticketDAL.GetById(id);
            if (ticket == null)
                return HttpNotFound();

            if (CurrentRole != "Administrator" && CurrentRole != "Support Executive")
                return RedirectToAction("AccessDenied", "Auth");

            var model = new TicketAssignViewModel
            {
                TicketId = ticket.TicketId,
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                SupportExecutives = GetSupportExecutives()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(TicketAssignViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SupportExecutives = GetSupportExecutives();
                return View(model);
            }

            try
            {
                ticketDAL.Assign(model.TicketId, model.AssignedTo, CurrentUserId, model.ChangeReason);
                TempData["SuccessMessage"] = "Ticket assigned successfully.";
                return RedirectToAction("Details", new { id = model.TicketId });
            }
            catch (Exception)
            {
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
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, error = "Could not change status." });
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

        private System.Collections.Generic.List<UserListItem> GetSupportExecutives()
        {
            var ds = authDAL.GetSupportExecutives();
            return ds.Tables[0].Rows.Cast<DataRow>().Select(r => new UserListItem
            {
                UserId = Convert.ToInt32(r["UserId"]),
                FullName = r["FullName"].ToString(),
                Email = r["Email"].ToString()
            }).ToList();
        }
    }
}
