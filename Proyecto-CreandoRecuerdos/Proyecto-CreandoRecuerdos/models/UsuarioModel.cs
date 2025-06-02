using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class UsuarioModel
    {
        public long id { get; set; }
        public string nombre { get; set; }
        public int id_rol { get; set; } // 1=admin, 2=trabajador
        public string correo { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
    }
}