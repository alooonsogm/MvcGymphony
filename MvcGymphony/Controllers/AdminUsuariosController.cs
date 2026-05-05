using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Filters;
using MvcGymphony.Services;
using NugetGymphonyAGM.Models;

namespace MvcGymphony.Controllers
{
    [AuthorizeUsuarios]
    public class AdminUsuariosController : Controller
    {
        private ServiceUsuarios serviceUsuarios;
        private ServiceHorarioUsuario serviceHorarios;

        public AdminUsuariosController( ServiceUsuarios serviceUsuarios, ServiceHorarioUsuario serviceHorarios )
        {
            this.serviceUsuarios = serviceUsuarios;
            this.serviceHorarios = serviceHorarios;
        }

        public async Task<IActionResult> PanelUsuarios( string seccion )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo el administrador puede acceder.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if ( seccion == null )
                {
                    seccion = "socios";
                }
                ViewData["SECCIONACTIVA"] = seccion;

                AdminGestionUsuarios model = new AdminGestionUsuarios();
                model.Socios = await this.serviceUsuarios.GetSociosConEstadoAsync();
                model.Entrenadores = await this.serviceUsuarios.GetUsuariosPorRolAsync("Entrenador");
                return View(model);
            }
        }

        public IActionResult CrearSocio()
        {
            if ( User.IsInRole("Administrador") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo el administrador puede acceder.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearSocio( string email, string password, string nombre, string apellidos, string telefono, DateOnly fechaNacimiento, string dni, IFormFile foto )
        {
            try
            {
                bool success = await this.serviceUsuarios.CrearSocioAsync(email, password, nombre, apellidos, telefono, fechaNacimiento, dni, foto);

                if ( success )
                {
                    TempData["MENSAJE_EXITO"] = $"El socio {nombre} {apellidos} ha sido dado de alta correctamente.";
                    return RedirectToAction("PanelUsuarios", new { seccion = "socios" });
                }
                else
                {
                    ViewData["MENSAJE_ERROR"] = "No se pudo registrar al socio. Revisa los datos o inténtalo más tarde.";
                    return View();
                }
            }
            catch ( Exception ex )
            {
                ViewData["MENSAJE_ERROR"] = "Error inesperado al conectar con el servidor: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DarBajaSocio( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                var (success, message) = await this.serviceUsuarios.DarDeBajaSocioAsync(id);
                return Json(new { success = success, message = message });
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DarDeAltaSocio( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                var (success, message) = await this.serviceUsuarios.DarDeAltaSocioAsync(id);

                return Json(new { success = success, message = message });
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarSocio( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceUsuarios.EliminarSocioAsync(id);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar borrar todos los datos del socio." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        public IActionResult CrearEntrenador()
        {
            if ( User.IsInRole("Administrador") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo el administrador puede acceder.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearEntrenador( string email, string password, string nombre, string apellidos, string telefono, DateOnly fechaNacimiento, string dni, IFormFile foto, List<int> diasSemana, List<TimeOnly> horasInicio, List<TimeOnly> horasFin )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                TempData["MENSAJERESERVA"] = "Acceso denegado: Solo el administrador puede acceder.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                bool success = await this.serviceUsuarios.CrearEntrenadorAsync(email, password, nombre, apellidos, telefono, fechaNacimiento, dni, foto, diasSemana, horasInicio, horasFin);

                if ( success )
                {
                    TempData["MENSAJE_EXITO"] = $"El entrenador {nombre} {apellidos} ha sido configurado y contratado correctamente.";
                    return RedirectToAction("PanelUsuarios", new { seccion = "entrenadores" });
                }
                else
                {
                    ViewData["MENSAJE_ERROR"] = "No se pudo registrar al entrenador. Revisa los datos o inténtalo más tarde.";
                    return View();
                }
            }
            catch ( Exception ex )
            {
                ViewData["MENSAJE_ERROR"] = "Error inesperado al conectar con el servidor: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerHorariosEntrenador( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                List<HorarioEmpleados> horarios = await this.serviceHorarios.GetHorariosEntrenadorAsync(id);

                var datosFormateados = horarios.Select(h => new {
                    diaSemana = h.DiaSemana,
                    horaInicio = h.HoraInicio.ToString("HH:mm"),
                    horaFin = h.HoraFin.ToString("HH:mm")
                });

                return Json(new { success = true, data = datosFormateados });
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error al obtener la tabla de horarios: " + ex.Message });
            }
        }

        public async Task<IActionResult> ValidarBorradoEntrenador( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                var (success, hasSessions, sustitutos, message) = await this.serviceUsuarios.ValidarBorradoEntrenadorAsync(id);

                if ( success )
                {
                    return Json(new { success = true, hasSessions = hasSessions, sustitutos = sustitutos });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error al validar: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarEntrenador( int idBorrar, int? idSustituto )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                var (success, message) = await this.serviceUsuarios.EliminarEntrenadorAsync(idBorrar, idSustituto);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}
