using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Filters;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;

namespace TicketResolver.Controllers
{
    [RoleAuthorize(1)]
    public class UserController : Controller
    {
        private readonly AuthDAL authDAL = new AuthDAL();
        private readonly MasterDAL masterDAL = new MasterDAL();

        public ActionResult Index(string searchTerm, int? roleId, bool? isActive, int page = 1)
        {
            try
            {
                var model = BuildUserSearchModel(searchTerm, roleId, isActive, page);
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Index", "Failed to load user list", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserSearchViewModel input)
        {
            try
            {
                var model = BuildUserSearchModel(input.SearchTerm, input.RoleId, input.IsActive, input.PageNumber);
                return PartialView("_UserTable", model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Index", "Failed to load user list (AJAX)", ex);
                return Content("<div class='alert alert-danger m-3'>Error loading users.</div>");
            }
        }

        private UserSearchViewModel BuildUserSearchModel(string searchTerm, int? roleId, bool? isActive, int page)
        {
            var ds = authDAL.SearchUsers(searchTerm, roleId, isActive, page, 10);

            return new UserSearchViewModel
            {
                SearchTerm = searchTerm,
                RoleId = roleId,
                IsActive = isActive,
                PageNumber = page,
                PageSize = 10,
                TotalCount = ds.Tables[0].Rows.Count > 0 ? Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]) : 0,
                Results = ds.Tables[0].Rows.Cast<DataRow>().Select(r => new UserListItemViewModel
                {
                    UserId = Convert.ToInt32(r["UserId"]),
                    RoleId = Convert.ToInt32(r["RoleId"]),
                    RoleName = r["RoleName"].ToString(),
                    FirstName = r["FirstName"].ToString(),
                    LastName = r["LastName"].ToString(),
                    Email = r["Email"].ToString(),
                    Mobile = r["Mobile"].ToString(),
                    CreatedDate = Convert.ToDateTime(r["CreatedDate"]),
                    IsActive = Convert.ToBoolean(r["IsActive"])
                }).ToList(),
                Roles = masterDAL.GetRoles()
            };
        }

        [RoleAuthorize(1)]
        public ActionResult Create()
        {
            try
            {
                var model = new UserCreateViewModel
                {
                    Roles = masterDAL.GetRoles()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Create", "Failed to load create user form", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = masterDAL.GetRoles();
                return View(model);
            }

            try
            {
                var userId = authDAL.InsertUser(model.RoleId, model.FirstName, model.LastName, model.Email, model.Mobile);
                var hash = PasswordHelper.HashPassword(model.Password);
                authDAL.InsertUserCredential(userId, hash);
                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Create", $"Failed to create user {model.Email}", ex);
                ModelState.AddModelError("", ex.Message);
                model.Roles = masterDAL.GetRoles();
                return View(model);
            }
        }

        [RoleAuthorize(1)]
        public ActionResult Edit(int id)
        {
            try
            {
                var user = authDAL.GetUserById(id);
                if (user == null) return HttpNotFound();

                var model = new UserEditViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    RoleId = user.RoleId,
                    IsActive = user.IsActive,
                    Roles = masterDAL.GetRoles()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Edit", $"Failed to load edit user form for id={id}", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = masterDAL.GetRoles();
                return View(model);
            }

            try
            {
                authDAL.UpdateUser(model.UserId, model.RoleId, model.FirstName, model.LastName, model.Email, model.Mobile);
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.Edit", $"Failed to update user id={model.UserId}", ex);
                ModelState.AddModelError("", ex.Message);
                model.Roles = masterDAL.GetRoles();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id, bool isActive)
        {
            try
            {
                authDAL.SetActiveStatus(id, isActive);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                AppLogger.Error("UserController.ToggleActive", $"Failed to toggle active status for id={id}", ex);
                return Json(new { success = false });
            }
        }
    }
}
