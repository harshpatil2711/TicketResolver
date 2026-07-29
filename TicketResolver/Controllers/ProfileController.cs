using System;
using System.Security.Claims;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;

namespace TicketResolver.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AuthDAL authDAL = new AuthDAL();

        private int CurrentUserId => int.Parse(((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier).Value);

        public ActionResult Index()
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

        public ActionResult Edit()
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
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult ChangePassword()
        {
            return View();
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
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}
