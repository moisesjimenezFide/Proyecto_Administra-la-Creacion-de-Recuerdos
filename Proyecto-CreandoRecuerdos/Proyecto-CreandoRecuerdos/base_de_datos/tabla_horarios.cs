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
    
    public partial class tabla_horarios
    {
        public int id_horario { get; set; }
        public int id_usuario { get; set; }
        public string dia_semana { get; set; }
        public System.TimeSpan hora_entrada { get; set; }
        public System.TimeSpan hora_salida { get; set; }
        public Nullable<bool> activo { get; set; }
    
        public virtual tabla_usuarios tabla_usuarios { get; set; }
    }
}
