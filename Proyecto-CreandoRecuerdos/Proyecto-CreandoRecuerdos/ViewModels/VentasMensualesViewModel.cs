using System;
using System.Globalization;

namespace Proyecto_CreandoRecuerdos.ViewModels
{
    public class VentasMensualesViewModel
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal Total { get; set; }
        public string MesFormateado => $"{Anio}-{Mes:D2}";
    }
}

