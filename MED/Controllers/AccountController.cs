using System.Security.Claims;
using MED.Services;
using MED.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MED.Controllers;

public class AccountController(MedStore store) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View("~/Views/Home/Login.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string userType)
    {
        var user = store.Read(d =>
            d.Users.FirstOrDefault(u =>
                u.Email.Equals(
                    email?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
                && u.Role == userType
            )
        );

        if (user == null ||
            new PasswordHasher<MedUser>()
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password ?? ""
                ) == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid login details or user type.";

            return View("~/Views/Home/Login.cshtml");
        }

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role
            )
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            )
        );

        return RedirectToAction(
            "Index",
            user.Role == "Employee" ? "Notices" : "Customer"
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }
}