using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PathologyFormApp.Models;
using PathologyFormApp.ViewModels;
using System.Threading.Tasks;

namespace PathologyFormApp.Controllers
{
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Pathology");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        // Check user role and redirect accordingly
                        var roles = await _userManager.GetRolesAsync(user);
                        
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        
                        // Role-based redirect
                        if (roles.Contains("Doctor"))
                        {
                            return RedirectToAction("Index", "Pathology");
                        }
                        else if (roles.Contains("Nurse"))
                        {
                            return RedirectToAction("NurseForm", "PathologyForm"); // Nurses go directly to nurse form
                        }
                        else
                        {
                            return RedirectToAction("Index", "Pathology");
                        }
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // Temporary action to generate a password hash
        [HttpGet]
        public IActionResult GeneratePasswordHash(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password parameter is required.");
            }

            var hasher = new PasswordHasher<User>();
            var hashedPassword = hasher.HashPassword(new User { FullName = "temp", Role = "temp" }, password); // User object is not used for hashing itself, but the type is needed

            return Content($"Password for '{password}': {hashedPassword}");
        }

        // Action to show user information (for debugging)
        [HttpGet]
        public async Task<IActionResult> UserInfo()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user!);

            return Content($"User: {user?.UserName}\nFull Name: {user?.FullName}\nRole: {user?.Role}\nRoles: {string.Join(", ", roles)}");
        }
    }
} 