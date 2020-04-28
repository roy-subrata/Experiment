using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginModel { returnUrl = returnUrl });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            if (model.UserName == "roy" && model.Password == "111")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name,model.UserName),
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

            }
            else
                return Unauthorized();

            return LocalRedirect(model.returnUrl);
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { IsPersistent = false });
            return Redirect("/");
        }
    }
}