using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamax_SRL.Models
{
    public class Cobros
    {
        [Key]
        public int CobroId { get; set; }

        [Required(ErrorMessage = "Favor seleccionar un deudor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Favor seleccionar un deudor válido.")]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Clientes Cliente { get; set; }

        [Required(ErrorMessage = "Favor seleccionar un préstamo.")]
        public int PrestamoId { get; set; }

        [ForeignKey("PrestamoId")]
        public Prestamos Prestamo { get; set; }

        public decimal? Mora { get; set; }
        public decimal? ImportePagar { get; set; }
        public DateTime FechaInicio { get; set; }
    }
}