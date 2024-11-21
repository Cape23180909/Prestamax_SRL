using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prestamax_SRL.Models;
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
    [Range(1, int.MaxValue, ErrorMessage = "Favor seleccionar un préstamo válido.")]
    public int PrestamoId { get; set; }

    [ForeignKey("PrestamoId")]
    public Prestamos Prestamo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "La mora no puede ser un valor negativo.")]
    public decimal? Mora { get; set; }

    [Required(ErrorMessage = "El importe a pagar es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El importe a pagar debe ser mayor a cero.")]
    public decimal ImportePagar { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de cobro es obligatoria.")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(Cobros), nameof(ValidarFechaCobro))]
    public DateTime? FechaCobro { get; set; }

    // Metodo para FechaCobro
    public static ValidationResult ValidarFechaCobro(DateTime? fechaCobro, ValidationContext context)
    {
        var cobros = (Cobros)context.ObjectInstance;
        if (fechaCobro.HasValue && fechaCobro < cobros.FechaInicio)
        {
            return new ValidationResult("La fecha de cobro no puede ser anterior a la fecha de inicio.");
        }
        return ValidationResult.Success;
    }
}