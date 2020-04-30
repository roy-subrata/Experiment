using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
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
        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallBack"),
                Items ={
                    {"returnUrl",returnUrl}
                }
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [AllowAnonymous]

        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallBack()
        {
            var results = await HttpContext.AuthenticateAsync("Men");
            var externalClaims = results.Principal.Claims.ToList();
            var subjectClaim = externalClaims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier);
            var claims = new List<Claim> {
             new Claim(ClaimTypes.NameIdentifier,subjectClaim.Value.ToString()),
             new Claim(ClaimTypes.Name,"Roy-Subrata"),
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Men");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync("Men");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return LocalRedirect(results.Properties.Items["returnUrl"]);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { IsPersistent = false });
            return Redirect("/");
        }
    }
}
