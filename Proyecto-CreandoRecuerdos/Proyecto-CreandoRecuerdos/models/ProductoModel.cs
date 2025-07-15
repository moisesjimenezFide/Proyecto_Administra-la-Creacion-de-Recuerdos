using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class ProductoModel
    {
        public long id_producto { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int id_categoria { get; set; }
        public decimal precio_por_unidad { get; set; }
        public string img_url { get; set; }
        public bool EsRecomendado { get; set; }

    }
}