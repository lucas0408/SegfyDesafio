using System.ComponentModel.DataAnnotations;
using Segfy.Api.Models;

namespace Segfy.Api.Dtos;

public class ApoliceUpdateDto : IValidatableObject
{
    [Required]
    [RegularExpression(@"^[A-Za-z]{3}\d{4}$|^[A-Za-z]{3}\d[A-Za-z]\d{2}$", ErrorMessage = "Placa em formato inválido.")]
    public string PlacaVeiculo { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor do prêmio deve ser maior que zero.")]
    public decimal ValorPremio { get; set; }

    [Required]
    public DateTime DataInicioVigencia { get; set; }

    [Required]
    public DateTime DataFimVigencia { get; set; }

    [Required]
    public StatusApolice Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataFimVigencia <= DataInicioVigencia)
        {
            yield return new ValidationResult(
                "A data de término deve ser posterior à data de início.",
                new[] { nameof(DataFimVigencia) });
        }
    }
}
