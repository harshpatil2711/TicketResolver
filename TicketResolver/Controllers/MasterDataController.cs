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
                    Categories = masterDAL.GetCategories()
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
    }

    public class MasterDataViewModel
    {
        public System.Collections.Generic.List<TicketCategory> Categories { get; set; }
    }
}
