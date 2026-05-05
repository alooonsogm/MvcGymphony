using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Models;
using MvcGymphony.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MvcGymphony.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceAuth service;

        public ManagedController( ServiceAuth service )
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login( LoginModelDTO model )
        {
            var (token, mensajeError) = await this.service.LogInAsync(model.Email, model.Password);
            if ( token == null )
            {
                ViewData["MENSAJE"] = mensajeError;
                return View();
            }
            else
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

                string nombreUsuario = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                string rolUsuario = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                string idUsuario = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim("TOKEN", token));
                identity.AddClaim(new Claim(ClaimTypes.Name, nombreUsuario));
                identity.AddClaim(new Claim(ClaimTypes.Role, rolUsuario));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                });
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
