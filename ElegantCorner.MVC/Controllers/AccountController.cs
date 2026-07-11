using ElegantCorner.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElegantCorner.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager  = userManager;
            _signInManager = signInManager;
        }

        // ── AJAX: POST /Account/Login ─────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var result = await _signInManager.PasswordSignInAsync(user, req.Password, isPersistent: true, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { displayName = user.DisplayName, email = user.Email });
        }

        // ── AJAX: POST /Account/Register ──────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (req.Password != req.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            var existing = await _userManager.FindByEmailAsync(req.Email);
            if (existing != null)
                return BadRequest(new { message = "Email already exists" });

            var user = new AppUser
            {
                DisplayName = req.DisplayName,
                Email       = req.Email,
                UserName    = req.Email
            };

            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors.First().Description });

            await _signInManager.SignInAsync(user, isPersistent: true);
            return Ok(new { displayName = user.DisplayName, email = user.Email });
        }

        // ── AJAX: POST /Account/Logout ────────────────────────────────────────
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        // ── AJAX: GET /Account/CurrentUser ────────────────────────────────────
        // Called by JS on every page load to restore UI state without a page reload
        [HttpGet]
        public IActionResult CurrentUser()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return Ok(new { isAuthenticated = false });

            return Ok(new
            {
                isAuthenticated = true,
                displayName     = User.FindFirstValue(ClaimTypes.Name),
                email           = User.FindFirstValue(ClaimTypes.Email)
            });
        }
    }

    // ── Request models ────────────────────────────────────────────────────────
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string DisplayName, string Email, string Password, string ConfirmPassword);
}
