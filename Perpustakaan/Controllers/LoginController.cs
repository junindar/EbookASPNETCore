using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Perpustakaan.Models;
using Perpustakaan.ViewModels;

namespace Perpustakaan.Controllers
{
    public class LoginController : Controller
    {

        private readonly PustakaDbContext _context;
        public LoginController(PustakaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users.FindAsync(model.Username);
                    if (user != null)
                    {
                        if (user.Password == model.Password)
                        {
                            var claims = new[] { new Claim(ClaimTypes.Name, user.Username),
                                new Claim(ClaimTypes.Role, user.Role) };
                            
                            var claimIdentities = new ClaimsIdentity(claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimPrincipal = new ClaimsPrincipal(claimIdentities);
                            var authenticationManager = Request.HttpContext;

                            // Sign In.  
                            await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                claimPrincipal, new AuthenticationProperties() { IsPersistent = false });
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Password salah!");
                            return View(model);
                        }
                    }

                    ModelState.AddModelError("", "Username salah!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(model);
        }

        public async Task<IActionResult> LogOff()
        {
            HttpContext.Session.Clear();
            var authenticationManager = Request.HttpContext;
            await authenticationManager.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }


        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var curUsername = User.Identity.Name;
                    var user = await _context.Users.FirstOrDefaultAsync(u =>
                        u.Username.ToLower().Equals(curUsername.ToLower()));
                    if (user != null)
                    {
                        if (user.Password == model.OldPassword)
                        {
                            user.Password = model.NewPassword;
                            await _context.SaveChangesAsync();
                            ModelState.AddModelError("", "Password berhasil diganti.");

                        }
                        else
                        {
                            ModelState.AddModelError("", "Password lama salah.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username tidak dikenal.");
                    }
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }


      

    }
}