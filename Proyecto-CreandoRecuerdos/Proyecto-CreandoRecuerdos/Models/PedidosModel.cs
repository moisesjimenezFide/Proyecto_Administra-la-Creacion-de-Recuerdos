using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_CreandoRecuerdos.Models
{
    public class PedidosModel
    {
        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NombreCliente { get; set; }

        [RegularExpression(@"^\d{8}$", ErrorMessage = "Teléfono debe tener 8 dígitos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método de pago")]
        public string MetodoPago { get; set; } = "Efectivo";

        public bool ParaLlevar { get; set; }

        [EnsureMinimumOneProduct(ErrorMessage = "Debe agregar al menos un producto al pedido")]
        public List<ProductoPedidoModel> Productos { get; set; } = new List<ProductoPedidoModel>();
    }

    public class EnsureMinimumOneProduct : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var productos = value as List<ProductoPedidoModel>;
            return productos != null && productos.Count > 0;
        }
    }

    public class ProductoPedidoModel
    {
        public int IdProducto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(500, ErrorMessage = "Las personalizaciones no pueden exceder 500 caracteres")]
        public string Personalizacion { get; set; }
        public string Nombre { get; set; }
        public string ImagenUrl { get; set; }
    }
    public class ValoracionModel
    {
        public int IdPedido { get; set; }
        public int IdValoracion { get; set; }

        [Required(ErrorMessage = "La calificación es requerida")]
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int Calificacion { get; set; }

        [StringLength(500, ErrorMessage = "Los comentarios no pueden exceder 500 caracteres")]
        public string Comentarios { get; set; }
        public DateTime Fecha { get; set; }

        public string NumeroPedido { get; set; }
    }

    public class PagoModel
    {
        [Required(ErrorMessage = "El método de pago es requerido")]
        public string MetodoPago { get; set; }

        // Campos para tarjeta
        [CreditCard(ErrorMessage = "Número de tarjeta inválido")]
        [RequiredIf("MetodoPago", "Tarjeta", ErrorMessage = "Número de tarjeta es requerido")]
        public string NumeroTarjeta { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$",
            ErrorMessage = "Formato de fecha inválido (MM/AA)")]
        [RequiredIf("MetodoPago", "Tarjeta", ErrorMessage = "Fecha de expiración es requerida")]
        public string FechaExpiracion { get; set; }

        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV debe tener 3 o 4 dígitos")]
        [RequiredIf("MetodoPago", "Tarjeta", ErrorMessage = "CVV es requerido")]
        public string CVV { get; set; }

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Monto debe ser mayor a cero")]
        [RequiredIf("MetodoPago", "Efectivo", ErrorMessage = "Monto recibido es requerido")]
        public decimal? MontoRecibido { get; set; }

        public decimal? Cambio { get; set; }

        [RegularExpression(@"^\d{8}$", ErrorMessage = "Teléfono SINPE debe tener 8 dígitos")]
        [RequiredIf("MetodoPago", "Sinpe", ErrorMessage = "Teléfono SINPE es requerido")]
        public string TelefonoSinpe { get; set; }

        public class RequiredIfAttribute : ValidationAttribute
        {
            private string PropertyName { get; set; }
            private object DesiredValue { get; set; }

            public RequiredIfAttribute(string propertyName, object desiredValue)
            {
                PropertyName = propertyName;
                DesiredValue = desiredValue;
            }

            protected override ValidationResult IsValid(object value, ValidationContext context)
            {
                var instance = context.ObjectInstance;
                var type = instance.GetType();
                var propertyValue = type.GetProperty(PropertyName)?.GetValue(instance, null);

                if (propertyValue?.ToString() == DesiredValue.ToString() && value == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
        }
    }

    public class PedidoPagoCompletoModel
    {
        // Datos del cliente
        public string NombreCliente { get; set; }
        public string Telefono { get; set; }
        public bool ParaLlevar { get; set; }
        public DateTime Fecha { get; set; }

        // Productos
        public List<ProductoPedidoModel> Productos { get; set; } = new List<ProductoPedidoModel>();

        // Totales
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }

        // Datos de pago
        public string MetodoPago { get; set; }
        public string TelefonoSinpe { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal Cambio { get; set; }

        // Tarjeta
        public string NumeroTarjeta { get; set; }
        public string FechaExpiracion { get; set; }
        public string CVV { get; set; }
    }

    public class ConfirmacionPedidoViewModel
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; }
        public string Pin { get; set; }
        public int TiempoEstimado { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public string TelefonoSinpe { get; set; }
        public List<ProductoPedidoModel> Productos { get; set; }
        public string TelefonoCliente { get; set; }
        public string TelefonoPedido { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoMostrado
        {
            get
            {
                if (MetodoPago.Equals("Sinpe", StringComparison.OrdinalIgnoreCase))
                {
                    // Para SINPE
                    return TelefonoSinpe;
                }

                // Para otros métodos, priorizar el teléfono específico del pedido
                return !string.IsNullOrEmpty(TelefonoPedido) ? TelefonoPedido : TelefonoCliente ?? "N/A";
            }
        }
    }

    public class DetallePedidoModel
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; }
        public string NombreCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public string TelefonoPedido { get; set; }
        public string TelefonoSinpe { get; set; }
        public string MetodoPago { get; set; }
        public bool ParaLlevar { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
        public string Pin { get; set; }
        public int TiempoEstimado { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public List<ProductoPedidoModel> Productos { get; set; }
        public bool PuedeActualizarEstado { get; set; }
        public List<string> EstadosDisponibles { get; set; }
        public string TelefonoDisplay
        {
            get
            {
                if (MetodoPago.Equals("Sinpe", StringComparison.OrdinalIgnoreCase))
                {
                    return $"SINPE: {TelefonoSinpe}";
                }
                return TelefonoPedido ?? TelefonoCliente ?? "N/A";
            }
        }

        public bool MostrarDetalleSinpe =>
            MetodoPago.Equals("Sinpe", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrEmpty(TelefonoSinpe);

        public string ClaseEstado
        {
            get
            {
                switch (Estado)
                {
                    case "Pendiente": return "bg-warning text-dark";
                    case "En preparación": return "bg-info text-white";
                    case "Listo": return "bg-success text-white";
                    case "Entregado": return "bg-secondary text-white";
                    case "Cancelado": return "bg-danger text-white";
                    default: return "bg-light text-dark";
                }
            }
        }
    }

    public class PedidoClienteViewModel
    {
        public int IdPedido { get; set; }
        public string NumeroPedido { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public int TiempoEstimado { get; set; }
        public string Pin { get; set; }
        public string Notificacion { get; set; }
        public bool Valorado { get; set; }

        public string ClaseEstado
        {
            get
            {
                switch (Estado)
                {
                    case "Pendiente": return "bg-warning text-dark";
                    case "En preparación": return "bg-info text-white";
                    case "Listo": return "bg-success text-white";
                    case "Entregado": return "bg-secondary text-white";
                    case "Cancelado": return "bg-danger text-white";
                    default: return "bg-light text-dark";
                }
            }
        }
    }

    public class NotificacionPedidoModel
    {
        public int IdPedido { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
    }
}