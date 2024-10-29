using Prestamax_SRL.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Prestamos
{
    [Key]
    public int PrestamosId { get; set; }

    [Required(ErrorMessage = "Favor seleccionar un deudor.")]
    [Range(1, int.MaxValue, ErrorMessage = "Favor seleccionar un deudor válido.")]
    public int ClienteId { get; set; }

    [ForeignKey("ClienteId")]
    public Clientes Cliente { get; set; }

    public decimal MontoPrestado { get; set; }

    public decimal Interes { get; set; }

    public int Cuotas { get; set; }

    public string FormaPago { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }

    public decimal? MontoCuota { get; set; }

    public decimal? TotalInteres { get; set; }

    public decimal? MontoTotalPagar { get; set; }
}
