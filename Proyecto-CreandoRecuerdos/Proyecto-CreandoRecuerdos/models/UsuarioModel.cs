using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class UsuarioModel
    {
        public long id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; }

        public int id_rol { get; set; } 

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string correo { get; set; }

        public bool activo { get; set; }

        public DateTime fecha_creacion { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string contrasenna { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string telefono { get; set; }

        public List<SelectListItem> RolesDisponibles { get; set; }

        public int? id_cliente { get; set; }
        public string apellido { get; set; }
    }
}