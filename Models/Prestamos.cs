using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamax_SRL.Models
{
    public class Prestamos
    {
        [Key]
        public int PrestamosId { get; set; }

        [Required(ErrorMessage = "Favor seleccionar un deudor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Favor seleccionar un deudor válido.")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Clientes Cliente { get; set; }

        [Required(ErrorMessage = "El monto prestado es obligatorio.")]
        [Range(1, (double)decimal.MaxValue, ErrorMessage = "El monto prestado debe ser mayor a 0.")]
        public decimal MontoPrestado { get; set; }

        [Required(ErrorMessage = "El interés es obligatorio.")]
        [Range(0.1, 100, ErrorMessage = "El interés debe estar entre 0.1% y 100%.")]
        public decimal Interes { get; set; }

        public int? Cuotas { get; set; }

        [Required(ErrorMessage = "La forma de pago es obligatoria.")]
        [StringLength(50, ErrorMessage = "La forma de pago no puede exceder los 50 caracteres.")]
        public string FormaPago { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }
        public DateTime? FechaInicio { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El monto por cuota debe ser un valor positivo.")]
        public decimal? MontoCuota { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El total de interés debe ser un valor positivo.")]
        public decimal? TotalInteres { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El monto total a pagar debe ser un valor positivo.")]
        public decimal? MontoTotalPagar { get; set; }
        public decimal? Saldo { get; set; }

        public virtual ICollection<Cobros> Cobros { get; set; } // Relación con Cobros
    }
}