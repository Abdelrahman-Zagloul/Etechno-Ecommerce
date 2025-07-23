namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public UserController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = SD.AdminRole)]
        public IActionResult Dashboard()
        {
            var orderHeaders = _context.OrderHeaders.AsNoTracking().Select(o => new { o.OrderStatus, o.PaymentStatus, o.TotalPrice }).ToList();
            var totalOrders = orderHeaders.Count;

            var loginGroups = _context.UserLogins
                    .AsNoTracking().GroupBy(x => x.LoginProvider)
                    .Select(g => new { Provider = g.Key, Count = g.Count() })
                    .ToDictionary(g => g.Provider, g => g.Count);

            var orderStatusCounts = orderHeaders.GroupBy(x => x.OrderStatus)
                                                .ToDictionary(g => g.Key, g => g.Count());
            var paymentStatusCounts = orderHeaders.GroupBy(x => x.PaymentStatus)
                                                  .ToDictionary(g => g.Key, g => g.Count());


            var adminDashboardViewModel = new AdminDashboardViewModel
            {
                TotalSalals = orderHeaders.Where(x => x.PaymentStatus == PaymentStatus.Approved).Sum(x => x.TotalPrice),
                TotalOrder = totalOrders,
                NumberOfCategory = _context.Categories.Count(),
                NumberOfProduct = _context.Products.Count(),

                TotalUser = _context.Users.Count(),
                FacebookUsers = loginGroups.ContainsKey("Facebook") ? loginGroups["Facebook"] : 0,
                GithubUsers = loginGroups.ContainsKey("GitHub") ? loginGroups["GitHub"] : 0,
                GoogleUsers = loginGroups.ContainsKey("Google") ? loginGroups["Google"] : 0,
                LinkedInUsers = loginGroups.ContainsKey("LinkedIn") ? loginGroups["LinkedIn"] : 0,
                MicrosoftUsers = loginGroups.ContainsKey("Microsoft") ? loginGroups["Microsoft"] : 0,

                PendingPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Pending, totalOrders),
                ApprovedPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Approved, totalOrders),
                InProcessPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Processing, totalOrders),
                ShippedPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Shipped, totalOrders),
                DeliveredPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Delivered, totalOrders),
                CancelledPercentOrderStatus = GetPercent(orderStatusCounts, OrderStatus.Cancelled, totalOrders),

                PendingPercentPaymentStatus = GetPercent(paymentStatusCounts, PaymentStatus.Pending, totalOrders),
                ApprovedPercentPaymentStatus = GetPercent(paymentStatusCounts, PaymentStatus.Approved, totalOrders),
                RefundedPercentPaymentStatus = GetPercent(paymentStatusCounts, PaymentStatus.Refunded, totalOrders),
                RejectedPercentPaymentStatus = GetPercent(paymentStatusCounts, PaymentStatus.Rejected, totalOrders)
            };

            return View(nameof(Dashboard), adminDashboardViewModel);
        }

        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public IActionResult Index()
        {
            return View(nameof(Index));
        }
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Role = roles.FirstOrDefault() ?? "Customer"
            };

            return View("Profile", model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "User not found.";
                return View("ChangePassword");
            }

            var account = new AccountForChangePassword { Id = user.Id, Email = user.Email };
            return View("ChangePassword", account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveChangePassword(AccountForChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id!);
                if (user == null)
                {
                    TempData["error"] = "User not found.";
                    return View("ChangePassword", model);
                }
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["success"] = "Password Changed Successfully";
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
                TempData["error"] = "Error When Change Password.";
            }
            return View("ChangePassword", model);
        }
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null || roles == null)
                return NotFound();
            if (user.Email == SD.AdminEmail)
            {
                TempData["error"] = "Can't Update Master Admin";
                return RedirectToAction(nameof(Index));
            }
            var updateAccountViewModel = new UpdateAccountViewModel()
            {
                Id = id,
                Email = user.Email,
                UserName = user.UserName.Split("@")[0],
                Role = roles.FirstOrDefault(),
                RoleList = new List<SelectListItem>
                {
                    new SelectListItem { Text =SD.AdminRole,Value=SD.AdminRole },
                    new SelectListItem { Text =SD.CustomerRole,Value=SD.CustomerRole }
                },
            };

            return View(updateAccountViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    TempData["error"] = "Failed to remove existing roles.";
                    return RedirectToAction(nameof(Index));
                }
                var addResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (addResult.Succeeded)
                {
                    TempData["success"] = "Update Role Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            model.RoleList = new List<SelectListItem>
                {
                    new SelectListItem { Text = SD.AdminRole, Value = SD.AdminRole },
                    new SelectListItem { Text = SD.CustomerRole, Value = SD.CustomerRole }
                };
            TempData["error"] = "Error in Update Account";
            return View(model);
        }


        #region API CALL
        [HttpGet]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> GetAll()
        {
            var allUsers = _userManager.Users.ToList();
            var userList = new List<AccountViewModel>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new AccountViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName.Split('@')[0]!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Role = roles.FirstOrDefault() ?? "Customer",
                    isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.Now
                });
            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }
            if (user.Email == SD.AdminEmail)
            {
                return Json(new { success = false, message = "Can't Lock Master Admin" });
            }
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(user);
            return Json(new { success = true, message = "Account has been locked." });
        }
        [HttpPost]
        public async Task<IActionResult> UnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, message = "Account has been unlocked." });
        }
        #endregion
        #region remote
        public async Task<IActionResult> CheckPassword(string oldPassword, string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return Json(false);

            bool isTruePassword = await _userManager.CheckPasswordAsync(user, oldPassword);
            return Json(isTruePassword);
        }
        public IActionResult CheckNewPassword(string newPassword, string oldPassword)
        {
            bool isMatch = newPassword == oldPassword;
            return Json(!isMatch);
        }
        #endregion

        private int GetPercent<TKey>(Dictionary<TKey, int> dict, TKey key, int total)
        {
            if (total == 0) return 0;
            return dict.ContainsKey(key) ? dict[key] * 100 / total : 0;
        }
    }
}
