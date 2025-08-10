using System;
using System.Collections.Generic;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class ActividadModel
    {
        public int IdActividad { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string TipoAccion { get; set; }
        public string TablaAfectada { get; set; }
        public int? IdRegistroAfectado { get; set; }
        public Dictionary<string, object> ValoresAnteriores { get; set; }
        public Dictionary<string, object> ValoresNuevos { get; set; }
        public string IpSolicitud { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
    }

    public class FiltroActividadesModel
    {
        public int? IdUsuario { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TipoAccion { get; set; }
        public string TablaAfectada { get; set; }
        public int? IdRegistroAfectado { get; set; }
    }
}