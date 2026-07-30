using System;
using System.Linq;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Filters;
using TicketResolver.Helpers;
using TicketResolver.Models;

namespace TicketResolver.Controllers
{
    [RoleAuthorize(1)]
    public class MasterDataController : Controller
    {
        private readonly MasterDAL masterDAL = new MasterDAL();

        public ActionResult Index()
        {
            try
            {
                var model = new MasterDataViewModel
                {
                    Categories = masterDAL.GetCategories(),
                    Priorities = masterDAL.GetPriorities(),
                    Statuses = masterDAL.GetStatuses()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("MasterDataController.Index", "Failed to load master data", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(string categoryName)
        {
            try { masterDAL.InsertCategory(categoryName); TempData["Success"] = "Category added."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.AddCategory", "Failed to add category", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCategory(int categoryId, string categoryName)
        {
            try { masterDAL.UpdateCategory(categoryId, categoryName); TempData["Success"] = "Category updated."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.UpdateCategory", "Failed to update category", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            try { masterDAL.DeleteCategory(id); return Json(new { success = true }); }
            catch (Exception ex) { AppLogger.Error("MasterDataController.DeleteCategory", "Failed to delete category", ex); return Json(new { success = false, error = "Could not delete." }); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPriority(string priorityName, int sequence)
        {
            try { masterDAL.InsertPriority(priorityName, sequence); TempData["Success"] = "Priority added."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.AddPriority", "Failed to add priority", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePriority(int priorityId, string priorityName, int sequence)
        {
            try { masterDAL.UpdatePriority(priorityId, priorityName, sequence); TempData["Success"] = "Priority updated."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.UpdatePriority", "Failed to update priority", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePriority(int id)
        {
            try { masterDAL.DeletePriority(id); return Json(new { success = true }); }
            catch (Exception ex) { AppLogger.Error("MasterDataController.DeletePriority", "Failed to delete priority", ex); return Json(new { success = false, error = "Could not delete." }); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStatus(string statusName, bool isTerminalState)
        {
            try { masterDAL.InsertStatus(statusName, isTerminalState); TempData["Success"] = "Status added."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.AddStatus", "Failed to add status", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int statusId, string statusName, bool isTerminalState)
        {
            try { masterDAL.UpdateStatus(statusId, statusName, isTerminalState); TempData["Success"] = "Status updated."; }
            catch (Exception ex) { AppLogger.Error("MasterDataController.UpdateStatus", "Failed to update status", ex); TempData["Error"] = ex.Message; }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStatus(int id)
        {
            try { masterDAL.DeleteStatus(id); return Json(new { success = true }); }
            catch (Exception ex) { AppLogger.Error("MasterDataController.DeleteStatus", "Failed to delete status", ex); return Json(new { success = false, error = "Could not delete." }); }
        }
    }

    public class MasterDataViewModel
    {
        public System.Collections.Generic.List<TicketCategory> Categories { get; set; }
        public System.Collections.Generic.List<TicketPriority> Priorities { get; set; }
        public System.Collections.Generic.List<TicketStatus> Statuses { get; set; }
    }
}
