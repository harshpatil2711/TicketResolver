using System;
using System.Security.Claims;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Filters;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;

namespace TicketResolver.Controllers
{
    [RoleAuthorize]
    public class ProfileController : Controller
    {
        private readonly AuthDAL authDAL = new AuthDAL();

        private int CurrentUserId => int.Parse(((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value);

        public ActionResult Index()
        {
            try
            {
                var user = authDAL.GetUserById(CurrentUserId);
                if (user == null) return HttpNotFound();

                var model = new ProfileIndexViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile,
                    CreatedDate = user.CreatedDate
                };

                var roleClaim = ((ClaimsPrincipal)User).FindFirst(ClaimTypes.Role)?.Value;
                model.RoleName = roleClaim ?? "Employee";

                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("ProfileController.Index", "Failed to load profile", ex);
                return View("Error");
            }
        }

        public ActionResult Edit()
        {
            try
            {
                var user = authDAL.GetUserById(CurrentUserId);
                if (user == null) return HttpNotFound();

                var model = new ProfileEditViewModel
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Mobile = user.Mobile
                };

                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("ProfileController.Edit", "Failed to load profile edit form", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var current = authDAL.GetUserById(CurrentUserId);
                authDAL.UpdateUser(CurrentUserId, current.RoleId, model.FirstName, model.LastName, model.Email, model.Mobile);
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLogger.Error("ProfileController.Edit", "Failed to update profile", ex);
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult ChangePassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                AppLogger.Error("ProfileController.ChangePassword", "Failed to load change password page", ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("", "Both fields are required.");
                return View();
            }

            if (newPassword.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters.");
                return View();
            }

            try
            {
                var hash = authDAL.GetPasswordHashByUserId(CurrentUserId);
                if (!PasswordHelper.VerifyPassword(currentPassword, hash))
                {
                    ModelState.AddModelError("", "Current password is incorrect.");
                    return View();
                }

                authDAL.UpdatePassword(CurrentUserId, PasswordHelper.HashPassword(newPassword));
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                AppLogger.Error("ProfileController.ChangePassword", "Failed to change password", ex);
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}
