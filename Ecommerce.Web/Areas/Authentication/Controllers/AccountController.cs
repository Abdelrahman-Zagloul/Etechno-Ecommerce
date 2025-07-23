
using System.Text.RegularExpressions;

namespace Ecommerce.Web.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.IsInRole(SD.AdminRole))
            {
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Admin", Value = "Admin" },
                    new SelectListItem { Text = "Customer", Value = "Customer" }
                };
                var registerViewModel = new RegisterViewModel { RolesList = roleList };
                return View(nameof(Register), registerViewModel);
            }
            return View(nameof(Register));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please make sure all fields are valid.";
                return View(nameof(Register), model);
            }

            bool isAdmin = User.FindFirstValue(ClaimTypes.Role) == SD.AdminRole;
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (isAdmin)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["success1"] = "Account Created successfully!";
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }

                await _userManager.AddToRoleAsync(user, SD.CustomerRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["success1"] = "Account Created successfully!";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                TempData["error"] = "Failed to create account.";
                return View(nameof(Register), model);
            }
        }
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["error"] = "Email or password is incorrect!";
                    return View("Login", model);
                }
                if (user.LockoutEnd >= DateTime.Now)
                {
                    TempData["warning"] = "Email Is Locked, Try Latter";
                    return View("Login", model);

                }
                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    TempData["success1"] = "Login Successfully!";
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    if (await _userManager.IsInRoleAsync(user, SD.AdminRole))
                    {
                        TempData["success"] = "Welcome Admin";
                        return RedirectToAction("Dashboard", "User", new { area = "Admin" });
                    }
                    else
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                TempData["error"] = "Email or password is incorrect!";
            }
            return View("Login", model);
        }

        #region Generic External Login 
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { area = "Authentication", returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                TempData["error"] = $"Login failed: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["error"] = "Error while Creating account.";
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                TempData["success1"] = $"Login with {info.LoginProvider} successful";

                var userSignedIn = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return RedirectToDashboardOrHome(userSignedIn!, returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@{info.LoginProvider}.com";
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];
            var userName = await GenerateUniqueUserNameAsync(name);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    TempData["error"] = "Error while creating user.";
                    return RedirectToAction(nameof(Login));
                }
                await _userManager.AddToRoleAsync(user, SD.CustomerRole);
            }

            var alreadyLinked = (await _userManager.GetLoginsAsync(user))
                .Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey);

            if (!alreadyLinked)
            {
                var linkResult = await _userManager.AddLoginAsync(user, info);
                if (!linkResult.Succeeded)
                {
                    TempData["error"] = "Error while linking external login.";
                    return RedirectToAction(nameof(Login));
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["success1"] = $"Login with {info.LoginProvider} successful";
            return RedirectToDashboardOrHome(user, returnUrl);
        }

        

        #endregion

        #region ForgetPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(nameof(ForgotPassword));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { token = token, email = model.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"<h3>Click <a href='{callbackUrl}'>here</a> to reset your password.</h3>"
            );

            return RedirectToAction("ForgotPasswordConfirmation");
        }
        public IActionResult ForgotPasswordConfirmation()
        {
            return View(nameof(ForgotPasswordConfirmation));
        }

        #endregion

        #region Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Login", "Account", new { area = "Authentication" });

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction(nameof(ResetPasswordConfirmation));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        public IActionResult ResetPasswordConfirmation()
        {
            return View(nameof(ResetPasswordConfirmation));
        }

        #endregion
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            HttpContext.Session.SetInt32(SD.CardNumber, 0);
            TempData["success"] = "Logout Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> IsUserNameInUse(string UserName, string Id)
        {

            var user = await _userManager.FindByNameAsync(UserName);

            if (user == null)
                return Json(true);

            if (user.Id == Id)
                return Json(true);

            return Json(false);


        }
        public async Task<IActionResult> IsEmailInUse(string Email)
        {

            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return Json(false);
            }
            return Json(true);


        }
        private async Task<string> GenerateUniqueUserNameAsync(string rawName)
        {
            var cleaned = Regex.Replace(rawName, @"[^a-zA-Z0-9]", "");

            if (string.IsNullOrWhiteSpace(cleaned))
                cleaned = "user" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string finalName = cleaned;
            int i = 1;

            while (await _userManager.FindByNameAsync(finalName) != null)
            {
                finalName = $"{cleaned}{i}";
                i++;
            }
            return finalName;
        }

        private IActionResult RedirectToDashboardOrHome(ApplicationUser user, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                return LocalRedirect(returnUrl);

            if (_userManager.IsInRoleAsync(user, SD.AdminRole).Result)
                return RedirectToAction("Dashboard", "User", new { area = "Admin" });

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

    }
}