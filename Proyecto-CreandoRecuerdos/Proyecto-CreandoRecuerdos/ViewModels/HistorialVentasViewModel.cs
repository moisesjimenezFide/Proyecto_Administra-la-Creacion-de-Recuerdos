using System;

namespace Proyecto_CreandoRecuerdos.ViewModels
{
    public class HistorialVentasViewModel
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Cliente { get; set; }
        public string Usuario { get; set; }
    }
}
