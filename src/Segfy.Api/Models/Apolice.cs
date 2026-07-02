namespace Segfy.Api.Models;

public class Apolice
{
    public int Id { get; set; }
    public string NumeroApolice { get; set; } = string.Empty;
    public string CpfCnpjSegurado { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public decimal ValorPremio { get; set; }
    public DateTime DataInicioVigencia { get; set; }
    public DateTime DataFimVigencia { get; set; }
    public StatusApolice Status { get; set; }
}
