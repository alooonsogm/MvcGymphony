using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Filters;
using MvcGymphony.Services;
using System.Security.Claims;

namespace MvcGymphony.Controllers
{
    [AuthorizeUsuarios]
    public class GraficosDatosController : Controller
    {
        private ServiceUsuarios serviceUsuarios;
        private ServiceRegistroAforo serviceRegistro;

        public GraficosDatosController( ServiceUsuarios serviceUsuarios, ServiceRegistroAforo serviceRegistro )
        {
            this.serviceUsuarios = serviceUsuarios;
            this.serviceRegistro = serviceRegistro;
        }

        public async Task<IActionResult> PanelEstadisticas( string seccion )
        {
            if ( User.IsInRole("Socio") == true )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Área exclusiva para la plantilla del gimnasio.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if ( seccion == null )
                {
                    seccion = "evolucion";
                }
                ViewData["SECCIONACTIVA"] = seccion;

                if ( seccion == "evolucion" )
                {
                    ViewData["EVOLUCION"] = await this.serviceUsuarios.GetEvolucionSociosAsync();
                }
                else if ( seccion == "horas" )
                {
                    ViewData["HORASPICO"] = await this.serviceRegistro.GetHorasPicoAsync();
                }
                return View();
            }
        }

        public async Task<IActionResult> AsistenciaSocio()
        {
            if ( User.IsInRole("Socio") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Esta vista es exclusiva para el seguimiento de socios.";
                return RedirectToAction("Index", "Home");
            }

            List<string> diasAsistencia = await this.serviceUsuarios.GetMisDiasAsistenciaAsync();
            ViewData["ASISTENCIA"] = diasAsistencia;
            return View();
        }
    }
}
