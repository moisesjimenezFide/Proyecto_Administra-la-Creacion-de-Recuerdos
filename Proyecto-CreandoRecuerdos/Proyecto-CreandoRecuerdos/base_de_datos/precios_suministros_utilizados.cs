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
    
    public partial class precios_suministros_utilizados
    {
        public int id { get; set; }
        public Nullable<int> id_precio_final_sugerido { get; set; }
        public Nullable<int> id_suministro_utilizado { get; set; }
        public Nullable<int> cantidad { get; set; }
        public string unidad_de_medida { get; set; }
        public Nullable<decimal> costo_por_cantidad { get; set; }
        public Nullable<decimal> total_costo { get; set; }
    
        public virtual tabla_precios_finales_sugeridos tabla_precios_finales_sugeridos { get; set; }
        public virtual tabla_suministros tabla_suministros { get; set; }
    }
}
