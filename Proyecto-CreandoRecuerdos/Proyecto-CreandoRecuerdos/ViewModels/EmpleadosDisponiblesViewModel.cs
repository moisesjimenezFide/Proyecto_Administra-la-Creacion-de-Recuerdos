using System;

namespace Proyecto_CreandoRecuerdos.ViewModels
{
    public class EmpleadosDisponiblesViewModel
    {
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Correo { get; set; }
        public string Dia { get; set; }
        public TimeSpan Entrada { get; set; }
        public TimeSpan Salida { get; set; }
        public string NombreCompleto { get; set; }
        public string Rol { get; set; }
        public string Estado { get; set; }
    }
}
