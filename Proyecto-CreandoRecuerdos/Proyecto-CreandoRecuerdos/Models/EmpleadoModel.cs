using System;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class EmpleadoModel
    {
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [Range(1, 2, ErrorMessage = "Rol no válido")]
        public int idRol { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        public string correo { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public bool activo { get; set; }

        public DateTime fecha_creacion { get; set; }

        [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres")]
        public string contrasenna { get; set; }
    }
}