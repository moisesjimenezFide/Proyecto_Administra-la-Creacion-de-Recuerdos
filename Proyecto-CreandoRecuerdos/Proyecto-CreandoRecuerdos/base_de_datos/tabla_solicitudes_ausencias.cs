//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_CreandoRecuerdos.base_de_datos
{
    using System;
    using System.Collections.Generic;
    
    public partial class tabla_solicitudes_ausencias
    {
        public int id_solicitud { get; set; }
        public int id_usuario { get; set; }
        public System.DateTime fecha_inicio { get; set; }
        public System.DateTime fecha_fin { get; set; }
        public string motivo { get; set; }
        public string estado { get; set; }
        public string tipo { get; set; }
    
        public virtual tabla_usuarios tabla_usuarios { get; set; }
    }
}
