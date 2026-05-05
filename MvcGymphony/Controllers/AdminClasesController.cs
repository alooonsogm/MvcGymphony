using Microsoft.AspNetCore.Mvc;
using MvcGymphony.Filters;
using MvcGymphony.Models;
using MvcGymphony.Services;
using NugetGymphonyAGM.Models;

namespace MvcGymphony.Controllers
{
    [AuthorizeUsuarios]
    public class AdminClasesController : Controller
    {
        private ServiceSesiones serviceSesiones;
        private ServiceClases serviceClases;
        private ServiceUsuarios serviceUsuarios;
        private ServiceSalas serviceSalas;

        public AdminClasesController( ServiceSesiones serviceSesiones, ServiceClases serviceClases, ServiceUsuarios serviceUsuarios, ServiceSalas serviceSalas )
        {
            this.serviceSesiones = serviceSesiones;
            this.serviceClases = serviceClases;
            this.serviceUsuarios = serviceUsuarios;
            this.serviceSalas = serviceSalas;
        }

        public async Task<IActionResult> PanelControl( string seccion )
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
                    seccion = "clases";
                }
                ViewData["SECCIONACTIVA"] = seccion;

                AdminGestionClases model = new AdminGestionClases();
                model.Clases = await this.serviceClases.GetTodasClasesAsync();
                model.Salas = await this.serviceSalas.GetTodasSalasAsync();
                model.Sesiones = await this.serviceSesiones.GetTodasSesionesAsync();
                model.Entrenadores = await this.serviceUsuarios.GetTodosEntrenadoresAsync();
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearSala( [FromBody] Salas nuevaSala )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceSalas.CrearSalaAsync(nuevaSala);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar crear la sala." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarSala( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceSalas.EliminarSalaAsync(id);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No puedes borrar esta sala porque ya tiene sesiones programadas. Borra primero las sesiones."
                    });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarSala( [FromBody] Salas salaEditada )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceSalas.EditarSalaAsync(salaEditada);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar actualizar los datos de la sala." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearClase( [FromBody] Clases nuevaClase )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceClases.CrearClaseAsync(nuevaClase);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar crear la clase." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarClase( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceClases.EliminarClaseAsync(id);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "No puedes borrar esta clase porque ya tiene sesiones programadas. Borra primero las sesiones."
                    });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarClase( [FromBody] Clases claseEditada )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceClases.EditarClaseAsync(claseEditada);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar actualizar la clase." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearSesion( [FromBody] Sesion nuevaSesion )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                ValidarSesionDTO dtoValidacion = new ValidarSesionDTO
                {
                    Fecha = nuevaSesion.Fecha,
                    HoraInicio = nuevaSesion.HoraInicio,
                    HoraFin = nuevaSesion.HoraFin,
                    IdEntrenador = nuevaSesion.EntrenadorId,
                    IdSala = nuevaSesion.SalaId,
                    IdSesionActual = 0
                };

                var (esValida, mensaje) = await this.serviceSesiones.ValidarSesionAsync(dtoValidacion);

                if ( esValida == false )
                {
                    return Json(new { success = false, message = mensaje });
                }

                bool success = await this.serviceSesiones.CrearSesionAsync(nuevaSesion);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al guardar la sesión." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado al crear la sesión: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarSesion( [FromBody] Sesion sesionEditada )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                ValidarSesionDTO dtoValidacion = new ValidarSesionDTO
                {
                    Fecha = sesionEditada.Fecha,
                    HoraInicio = sesionEditada.HoraInicio,
                    HoraFin = sesionEditada.HoraFin,
                    IdEntrenador = sesionEditada.EntrenadorId,
                    IdSala = sesionEditada.SalaId,
                    IdSesionActual = sesionEditada.IdSesion
                };

                var (esValida, mensaje) = await this.serviceSesiones.ValidarSesionAsync(dtoValidacion);

                if ( esValida == false )
                {
                    return Json(new { success = false, message = mensaje });
                }

                bool success = await this.serviceSesiones.EditarSesionAsync(sesionEditada);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al actualizar la sesión." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado al actualizar: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EliminarSesion( int id )
        {
            if ( User.IsInRole("Administrador") == false )
            {
                return Json(new { success = false, message = "Acceso denegado: Acción no autorizada." });
            }

            try
            {
                bool success = await this.serviceSesiones.EliminarSesionAsync(id);

                if ( success )
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error del servidor al intentar eliminar la sesión." });
                }
            }
            catch ( Exception ex )
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }
    }
}
