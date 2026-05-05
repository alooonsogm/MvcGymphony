using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Filters;
using MvcGymphony.Services;
using QRCoder;
using System.Security.Claims;

namespace MvcGymphony.Controllers
{
    [AuthorizeUsuarios]
    public class AccesosController : Controller
    {
        private ServiceRegistroAforo serviceRegistro;

        public AccesosController( ServiceRegistroAforo serviceRegistro )
        {
            this.serviceRegistro = serviceRegistro;
        }

        public async Task<IActionResult> AforoActual()
        {
            try
            {
                int aforo = await this.serviceRegistro.GetAforoActualAsync();
                ViewData["AFORO"] = aforo;
            }
            catch
            {
                ViewData["AFORO"] = 0;
            }
            return View();
        }

        public IActionResult QRAcceso()
        {
            if ( User.IsInRole("Socio") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo los socios tienen acceso al QR.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                int idUser = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(idUser.ToString(), QRCodeGenerator.ECCLevel.H);
                Base64QRCode qrCode = new Base64QRCode(qrCodeData);
                string qrBase64 = qrCode.GetGraphic(20);
                ViewData["QR"] = qrBase64;
                return View();
            }
        }

        public IActionResult LectorCodigoQR()
        {
            if ( User.IsInRole("Administrador") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo el Administrador tiene acceso.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ControlAccesoQR( int idSocio )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado." });
            }

            try
            {
                var (success, operacion, message) = await this.serviceRegistro.RegistrarAccesoAsync(idSocio);

                if ( success )
                {
                    return Json(new { success = true, operacion = operacion });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error de lectura: " + ex.Message });
            }
        }
    }
}
