using System;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class SolicitudAusenciaModel
    {
        public int IdSolicitud { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Tipo { get; set; } // "Vacaciones" o "Permiso"
        public string Motivo { get; set; }
        public string Estado { get; set; } // "Pendiente", "Aprobado", "Rechazado"
    }
}