using Segfy.Api.Dtos;
using Segfy.Api.Models;
using Segfy.Api.Repositories;

namespace Segfy.Api.Services;

public class ApoliceService : IApoliceService
{
    private readonly IApoliceRepository _repository;

    public ApoliceService(IApoliceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApoliceResponseDto> CriarAsync(ApoliceCreateDto dto)
    {
        var numero = await GerarNumeroApoliceAsync();

        var apolice = new Apolice
        {
            NumeroApolice = numero,
            CpfCnpjSegurado = dto.CpfCnpjSegurado,
            PlacaVeiculo = dto.PlacaVeiculo.ToUpperInvariant(),
            ValorPremio = dto.ValorPremio,
            DataInicioVigencia = dto.DataInicioVigencia.Date,
            DataFimVigencia = dto.DataFimVigencia.Date,
            Status = StatusApolice.Ativa
        };

        await _repository.AddAsync(apolice);

        return MapToDto(apolice);
    }

    public async Task<ApoliceResponseDto?> ObterPorIdAsync(int id)
    {
        var apolice = await _repository.GetByIdAsync(id);
        if (apolice is null)
        {
            return null;
        }

        await AtualizarStatusSeExpiradaAsync(apolice);
        return MapToDto(apolice);
    }

    public async Task<IReadOnlyList<ApoliceResponseDto>> ListarAsync()
    {
        var apolices = await _repository.GetAllAsync();

        foreach (var apolice in apolices)
        {
            await AtualizarStatusSeExpiradaAsync(apolice);
        }

        return apolices.Select(MapToDto).ToList();
    }

    public async Task<ApoliceResponseDto?> AtualizarAsync(int id, ApoliceUpdateDto dto)
    {
        var apolice = await _repository.GetByIdAsync(id);
        if (apolice is null)
        {
            return null;
        }

        apolice.PlacaVeiculo = dto.PlacaVeiculo.ToUpperInvariant();
        apolice.ValorPremio = dto.ValorPremio;
        apolice.DataInicioVigencia = dto.DataInicioVigencia.Date;
        apolice.DataFimVigencia = dto.DataFimVigencia.Date;
        apolice.Status = dto.Status;

        await _repository.UpdateAsync(apolice);

        return MapToDto(apolice);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var apolice = await _repository.GetByIdAsync(id);
        if (apolice is null)
        {
            return false;
        }

        await _repository.DeleteAsync(apolice);
        return true;
    }

    public async Task<IReadOnlyList<ApoliceResponseDto>> ListarVencendoEm30DiasAsync()
    {
        var apolices = await _repository.GetVencendoEm30DiasAsync();
        return apolices.Select(MapToDto).ToList();
    }

    private async Task<string> GerarNumeroApoliceAsync()
    {
        var ano = DateTime.UtcNow.Year;
        var quantidade = await _repository.CountByYearAsync(ano);
        var sequencial = quantidade + 1;
        return $"SEG-{ano}-{sequencial:D4}";
    }

    private async Task AtualizarStatusSeExpiradaAsync(Apolice apolice)
    {
        if (apolice.Status == StatusApolice.Ativa && apolice.DataFimVigencia.Date < DateTime.Today)
        {
            apolice.Status = StatusApolice.Expirada;
            await _repository.UpdateAsync(apolice);
        }
    }

    private static ApoliceResponseDto MapToDto(Apolice apolice)
    {
        return new ApoliceResponseDto
        {
            Id = apolice.Id,
            NumeroApolice = apolice.NumeroApolice,
            CpfCnpjSegurado = apolice.CpfCnpjSegurado,
            PlacaVeiculo = apolice.PlacaVeiculo,
            ValorPremio = apolice.ValorPremio,
            DataInicioVigencia = apolice.DataInicioVigencia,
            DataFimVigencia = apolice.DataFimVigencia,
            Status = apolice.Status
        };
    }
}
