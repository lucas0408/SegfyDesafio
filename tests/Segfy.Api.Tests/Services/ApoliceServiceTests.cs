using Moq;
using Segfy.Api.Dtos;
using Segfy.Api.Models;
using Segfy.Api.Repositories;
using Segfy.Api.Services;
using Xunit;

namespace Segfy.Api.Tests.Services;

public class ApoliceServiceTests
{
    private readonly Mock<IApoliceRepository> _repositoryMock;
    private readonly ApoliceService _service;

    public ApoliceServiceTests()
    {
        _repositoryMock = new Mock<IApoliceRepository>();
        _service = new ApoliceService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarPrimeiroNumeroDoAno_QuandoNaoExistemApolices()
    {
        var ano = DateTime.UtcNow.Year;
        _repositoryMock.Setup(r => r.CountByYearAsync(ano)).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Apolice>())).ReturnsAsync((Apolice a) => a);

        var dto = new ApoliceCreateDto
        {
            CpfCnpjSegurado = "12345678900",
            PlacaVeiculo = "abc1234",
            ValorPremio = 150.50m,
            DataInicioVigencia = DateTime.Today,
            DataFimVigencia = DateTime.Today.AddMonths(12)
        };

        var resultado = await _service.CriarAsync(dto);

        Assert.Equal($"SEG-{ano}-0001", resultado.NumeroApolice);
        Assert.Equal("ABC1234", resultado.PlacaVeiculo);
        Assert.Equal(StatusApolice.Ativa, resultado.Status);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarNumeroSequencial_QuandoJaExistemApolicesNoAno()
    {
        var ano = DateTime.UtcNow.Year;
        _repositoryMock.Setup(r => r.CountByYearAsync(ano)).ReturnsAsync(7);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Apolice>())).ReturnsAsync((Apolice a) => a);

        var dto = new ApoliceCreateDto
        {
            CpfCnpjSegurado = "12345678900",
            PlacaVeiculo = "abc1234",
            ValorPremio = 150.50m,
            DataInicioVigencia = DateTime.Today,
            DataFimVigencia = DateTime.Today.AddMonths(12)
        };

        var resultado = await _service.CriarAsync(dto);

        Assert.Equal($"SEG-{ano}-0008", resultado.NumeroApolice);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoApoliceNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Apolice?)null);

        var resultado = await _service.ObterPorIdAsync(1);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveAtualizarStatusParaExpirada_QuandoDataFimJaPassou()
    {
        var apolice = new Apolice
        {
            Id = 1,
            NumeroApolice = "SEG-2025-0001",
            Status = StatusApolice.Ativa,
            DataFimVigencia = DateTime.Today.AddDays(-1)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(apolice);

        var resultado = await _service.ObterPorIdAsync(1);

        Assert.Equal(StatusApolice.Expirada, resultado!.Status);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Apolice>(a => a.Status == StatusApolice.Expirada)), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_NaoDeveAlterarStatus_QuandoApoliceCanceladaEstaVencida()
    {
        var apolice = new Apolice
        {
            Id = 1,
            NumeroApolice = "SEG-2025-0001",
            Status = StatusApolice.Cancelada,
            DataFimVigencia = DateTime.Today.AddDays(-1)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(apolice);

        var resultado = await _service.ObterPorIdAsync(1);

        Assert.Equal(StatusApolice.Cancelada, resultado!.Status);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Apolice>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarNull_QuandoApoliceNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Apolice?)null);

        var resultado = await _service.AtualizarAsync(99, new ApoliceUpdateDto
        {
            PlacaVeiculo = "ABC1234",
            DataInicioVigencia = DateTime.Today,
            DataFimVigencia = DateTime.Today.AddMonths(1)
        });

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarCampos_QuandoApoliceExiste()
    {
        var apolice = new Apolice { Id = 1, PlacaVeiculo = "OLD1234", ValorPremio = 100 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(apolice);

        var dto = new ApoliceUpdateDto
        {
            PlacaVeiculo = "new5678",
            ValorPremio = 200,
            DataInicioVigencia = DateTime.Today,
            DataFimVigencia = DateTime.Today.AddMonths(6),
            Status = StatusApolice.Cancelada
        };

        var resultado = await _service.AtualizarAsync(1, dto);

        Assert.Equal("NEW5678", resultado!.PlacaVeiculo);
        Assert.Equal(200, resultado.ValorPremio);
        Assert.Equal(StatusApolice.Cancelada, resultado.Status);
        _repositoryMock.Verify(r => r.UpdateAsync(apolice), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRetornarFalse_QuandoApoliceNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Apolice?)null);

        var resultado = await _service.RemoverAsync(1);

        Assert.False(resultado);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Apolice>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverApolice_QuandoApoliceExiste()
    {
        var apolice = new Apolice { Id = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(apolice);

        var resultado = await _service.RemoverAsync(1);

        Assert.True(resultado);
        _repositoryMock.Verify(r => r.DeleteAsync(apolice), Times.Once);
    }

    [Fact]
    public async Task ListarVencendoEm30DiasAsync_DeveRetornarApolicesMapeadas()
    {
        var apolices = new List<Apolice>
        {
            new() { Id = 1, NumeroApolice = "SEG-2026-0001", DataFimVigencia = DateTime.Today.AddDays(10) },
            new() { Id = 2, NumeroApolice = "SEG-2026-0002", DataFimVigencia = DateTime.Today.AddDays(20) }
        };

        _repositoryMock.Setup(r => r.GetVencendoEm30DiasAsync()).ReturnsAsync(apolices);

        var resultado = await _service.ListarVencendoEm30DiasAsync();

        Assert.Equal(2, resultado.Count);
        Assert.Contains(resultado, a => a.NumeroApolice == "SEG-2026-0001");
    }
}
