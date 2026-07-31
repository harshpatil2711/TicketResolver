using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketResolver.DAL;
using TicketResolver.Helpers;
using TicketResolver.ViewModels;

namespace TicketResolver.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthDAL authDAL = new AuthDAL();
        private readonly MasterDAL masterDAL = new MasterDAL();

        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                if (Request.Cookies["jwt_token"] != null)
                    return RedirectToAction("Index", "Home");
                return View();
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.Login", "Failed to load login page", ex);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return Request.IsAjaxRequest() ? Json(new { success = false, error = "Invalid form data." }) : (ActionResult)View(model);

            try
            {
                var user = authDAL.GetUserByEmail(model.Email);
                if (user == null)
                {
                    var err = "Invalid email or password.";
                    return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
                }

                if (!user.IsActive)
                {
                    var err = "Your account is pending admin approval.";
                    return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
                }

                var passwordHash = authDAL.GetPasswordHashByUserId(user.UserId);
                if (passwordHash == null || !PasswordHelper.VerifyPassword(model.Password, passwordHash))
                {
                    var err = "Invalid email or password.";
                    return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
                }

                var otp = OtpHelper.GenerateOtp();
                var expiry = DateTime.Now.AddMinutes(5);
                authDAL.InvalidatePreviousOtps(user.Email, "Login");
                authDAL.InsertOtpVerification(user.UserId, user.Email, otp, "Login", expiry);

                var userName = $"{user.FirstName} {user.LastName}";
                var template = System.IO.File.ReadAllText(Server.MapPath("~/EmailTemplates/OtpEmail.html"));
                EmailHelper.SendEmail(user.Email, "Your Login OTP Code", EmailTemplates.PopulateOtpTemplate(template, otp, "Login", userName));

                TempData["OtpUserId"] = user.UserId;
                var redirectUrl = Url.Action("VerifyOtp", new { email = user.Email, purpose = "Login" });
                return Request.IsAjaxRequest() ? Json(new { success = true, redirectUrl }) : (ActionResult)RedirectToAction("VerifyOtp", new { email = user.Email, purpose = "Login" });
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.Login", $"Failed login attempt for {model.Email}", ex);
                var err = "An error occurred. Please try again.";
                return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            try
            {
                var model = new RegisterViewModel
                {
                    Roles = GetSignupRoles()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.Register", "Failed to load register page", ex);
                return View(new RegisterViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = GetSignupRoles();
                return Request.IsAjaxRequest() ? Json(new { success = false, error = "Invalid form data." }) : (ActionResult)View(model);
            }

            try
            {
                var existingUser = authDAL.GetUserByEmail(model.Email);
                if (existingUser != null)
                {
                    var err = "Email already registered.";
                    model.Roles = GetSignupRoles();
                    return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
                }

                var otp = OtpHelper.GenerateOtp();
                var expiry = DateTime.Now.AddMinutes(5);
                authDAL.InvalidatePreviousOtps(model.Email, "Signup");
                authDAL.InsertOtpVerification(null, model.Email, otp, "Signup", expiry);

                var template = System.IO.File.ReadAllText(Server.MapPath("~/EmailTemplates/OtpEmail.html"));
                EmailHelper.SendEmail(model.Email, "Verify Your Email - OTP Code", EmailTemplates.PopulateOtpTemplate(template, otp, "Signup", model.FirstName));

                TempData["RegFirstName"] = model.FirstName;
                TempData["RegLastName"] = model.LastName;
                TempData["RegEmail"] = model.Email;
                TempData["RegMobile"] = model.Mobile;
                TempData["RegPassword"] = model.Password;
                TempData["RegRoleId"] = model.RoleId;

                var redirectUrl = Url.Action("VerifyOtp", new { email = model.Email, purpose = "Signup" });
                return Request.IsAjaxRequest() ? Json(new { success = true, redirectUrl }) : (ActionResult)RedirectToAction("VerifyOtp", new { email = model.Email, purpose = "Signup" });
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.Register", $"Failed registration for {model.Email}", ex);
                model.Roles = GetSignupRoles();
                var err = ex.Message.Contains("Email") ? "Email already registered." : "An error occurred. Please try again.";
                return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
            }
        }

        private System.Collections.Generic.List<TicketResolver.Models.TicketRole> GetSignupRoles()
        {
            return masterDAL.GetRoles().Where(r => r.RoleId != 1).ToList();
        }

        [HttpGet]
        public ActionResult VerifyOtp(string email, string purpose)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(purpose))
                    return RedirectToAction("Login");

                ViewBag.Email = email;
                ViewBag.Purpose = purpose;
                return View(new VerifyOtpViewModel { Email = email, Purpose = purpose });
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.VerifyOtp", "Failed to load verify OTP page", ex);
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
                return Request.IsAjaxRequest() ? Json(new { success = false, error = "Enter the OTP code." }) : (ActionResult)View(model);

            try
            {
                var result = authDAL.VerifyOtp(model.Email, model.OtpCode, model.Purpose);
                if (!result.IsValid)
                {
                    var err = "Invalid or expired OTP. Please try again.";
                    return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
                }

                authDAL.InvalidatePreviousOtps(model.Email, model.Purpose);

                if (model.Purpose == "Signup")
                {
                    var firstName = TempData["RegFirstName"]?.ToString();
                    var lastName = TempData["RegLastName"]?.ToString();
                    var email = TempData["RegEmail"]?.ToString();
                    var mobile = TempData["RegMobile"]?.ToString();
                    var password = TempData["RegPassword"]?.ToString();

                    if (string.IsNullOrEmpty(email))
                    {
                        var err = "Session expired. Please register again.";
                        return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)RedirectToAction("Register");
                    }

                    int roleId = Convert.ToInt32(TempData["RegRoleId"] ?? 3);
                    int userId = authDAL.InsertUser(roleId, firstName, lastName, email, mobile);
                    var passwordHash = PasswordHelper.HashPassword(password);
                    authDAL.InsertUserCredential(userId, passwordHash);

                    TempData["SuccessMessage"] = "Registration successful! An administrator will activate your account shortly.";
                    var redirectUrl = Url.Action("Login");
                    return Request.IsAjaxRequest() ? Json(new { success = true, redirectUrl }) : (ActionResult)RedirectToAction("Login");
                }
                else if (model.Purpose == "Login")
                {
                    var userIdObj = TempData["OtpUserId"];
                    int userId;
                    if (userIdObj == null || !int.TryParse(userIdObj.ToString(), out userId))
                    {
                        var user = authDAL.GetUserByEmail(model.Email);
                        if (user == null)
                            return RedirectToAction("Login");
                        userId = user.UserId;
                    }

                    var loggedInUser = authDAL.GetUserById(userId);
                    if (loggedInUser == null || !loggedInUser.IsActive)
                        return RedirectToAction("Login");

                    var roleName = loggedInUser.RoleId == 1 ? "Administrator" : loggedInUser.RoleId == 2 ? "Support Executive" : "Employee";
                    var accessToken = JwtHelper.GenerateAccessToken(loggedInUser.UserId, loggedInUser.Email, roleName);
                    var refreshToken = JwtHelper.GenerateRefreshToken();
                    var refreshTokenHash = JwtHelper.HashRefreshToken(refreshToken);
                    var expiryDate = DateTime.UtcNow.AddDays(JwtHelper.GetRefreshTokenExpiryDays());

                    authDAL.DeactivateAllRefreshTokens(loggedInUser.UserId);
                    authDAL.InsertRefreshToken(loggedInUser.UserId, refreshTokenHash, expiryDate);

                    var accessCookie = new HttpCookie("jwt_token", accessToken) { HttpOnly = true, Secure = false, Expires = expiryDate };
                    var refreshCookie = new HttpCookie("refresh_token", refreshToken) { HttpOnly = true, Secure = false, Expires = expiryDate };
                    Response.Cookies.Add(accessCookie);
                    Response.Cookies.Add(refreshCookie);

                    var redirectUrl = Url.Action("Index", "Home");
                    return Request.IsAjaxRequest() ? Json(new { success = true, redirectUrl }) : (ActionResult)RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.VerifyOtp", $"OTP verification failed for {model.Email}", ex);
                var err = "An error occurred. Please try again.";
                return Request.IsAjaxRequest() ? Json(new { success = false, error = err }) : (ActionResult)View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResendOtp(string email, string purpose)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(purpose))
                return Json(new { success = false });

            try
            {
                var otp = OtpHelper.GenerateOtp();
                var expiry = DateTime.Now.AddMinutes(5);
                authDAL.InvalidatePreviousOtps(email, purpose);
                authDAL.InsertOtpVerification(null, email, otp, purpose, expiry);

                string userName = email.Split('@')[0];
                string subject = purpose == "Login" ? "Your Login OTP Code" : "Verify Your Email - OTP Code";
                var template = System.IO.File.ReadAllText(Server.MapPath("~/EmailTemplates/OtpEmail.html"));
                EmailHelper.SendEmail(email, subject, EmailTemplates.PopulateOtpTemplate(template, otp, purpose, userName));

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.ResendOtp", $"Failed to resend OTP to {email}", ex);
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            if (Request.Cookies["jwt_token"] != null)
            {
                var token = Request.Cookies["jwt_token"].Value;
                try
                {
                    var principal = JwtHelper.ValidateToken(token);
                    var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        int userId = int.Parse(userIdClaim.Value);
                        authDAL.DeactivateAllRefreshTokens(userId);
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error("AuthController.Logout", "Failed to deactivate refresh tokens during logout", ex);
                }

                Response.Cookies["jwt_token"].Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies["refresh_token"].Expires = DateTime.UtcNow.AddDays(-1);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                AppLogger.Error("AuthController.AccessDenied", "Failed to load access denied page", ex);
                return View();
            }
        }
    }
}
