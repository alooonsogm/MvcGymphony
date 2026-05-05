using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Filters;
using MvcGymphony.Models;
using MvcGymphony.Services;
using NugetGymphonyAGM.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MvcGymphony.Controllers
{
    [AuthorizeUsuarios]
    public class HomeController : Controller
    {

        private ServiceSesiones service;
        private ServiceReservaSesiones serviceReserva;
        private ServiceUsuarios serviceUsuarios;
        private ServiceHorarioUsuario serviceHorario;

        public HomeController( ServiceSesiones service, ServiceReservaSesiones serviceReserva, ServiceUsuarios serviceUsuarios, ServiceHorarioUsuario serviceHorario )
        {
            this.service = service;
            this.serviceReserva = serviceReserva;
            this.serviceUsuarios = serviceUsuarios;
            this.serviceHorario = serviceHorario;
        }

        public async Task<IActionResult> Index()
        {
            List<DatosSesion> sesiones = await this.service.GetSesionesNuevasAsync();
            List<int> sesionesReservadas = new List<int>();

            if ( User.IsInRole("Socio") )
            {
                sesionesReservadas = await this.service.GetSesionesReservadasClienteNumeroAsync();
            }

            ViewData["SESIONES_RESERVADAS"] = sesionesReservadas;
            return View(sesiones);
        }

        public async Task<IActionResult> ReservarSesiones( int idSesion )
        {
            if ( User.IsInRole("Socio") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo los socios pueden reservar clases.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["MENSAJERESERVA"] = await this.serviceReserva.ReservarPlazaAsync(idSesion);

                DatosSesion sesion = await this.service.FindDatosSesionAsync(idSesion);
                if ( sesion != null )
                {
                    TempData["NOMBRECLASE"] = sesion.NombreClase;
                }

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> AnularSesiones( int idSesion )
        {
            if ( User.IsInRole("Socio") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo los socios pueden anular reservas.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["MENSAJERESERVA"] = await this.serviceReserva.AnularReservaAsync(idSesion);

                DatosSesion sesion = await this.service.FindDatosSesionAsync(idSesion);
                if ( sesion != null )
                {
                    TempData["NOMBRECLASE"] = sesion.NombreClase;
                }

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> GetSociosSesion( int idSesion )
        {
            if ( User.IsInRole("Socio") == true )
            {
                return Json(new List<object>());
            }
            else
            {
                List<Usuario> socios = await this.serviceUsuarios.GetUsuariosPorSesionAsync(idSesion);

                var resultado = socios.Select(s => new
                {
                    nombreCompleto = s.Nombre + " " + s.Apellidos,
                    email = s.Email
                }).ToList();

                return Json(resultado);
            }
        }

        public async Task<IActionResult> Perfil()
        {
            string rol = User.FindFirst(ClaimTypes.Role)?.Value;
            Usuario user = await this.serviceUsuarios.GetMiPerfilAsync();
            ViewData["ROL"] = rol;
            ViewData["PATHFOTO"] = user.RutaFoto;

            if ( rol == "Socio" )
            {
                List<DatosSesion> misReservas = await this.service.GetMisSesionesCompletasAsync();
                ViewData["MIS_RESERVAS"] = misReservas ?? new List<DatosSesion>();
            }
            else if ( rol == "Entrenador" )
            {
                List<HorarioEmpleados> misHorarios = await this.serviceHorario.GetHorariosDeEntrenadorPerfilAsync();
                ViewData["MIS_HORARIOS"] = misHorarios ?? new List<HorarioEmpleados>();
            }

            return View(user);
        }
    }
}
