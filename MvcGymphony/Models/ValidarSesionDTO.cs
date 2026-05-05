namespace MvcGymphony.Models
{
    public class ValidarSesionDTO
    {
        public DateOnly Fecha { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public int IdEntrenador { get; set; }
        public int IdSala { get; set; }
        public int? IdSesionActual { get; set; }
    }
}
