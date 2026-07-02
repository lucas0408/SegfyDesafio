using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Segfy.Api.Data;
using Segfy.Api.Models;
using Segfy.Api.Repositories;
using Xunit;

namespace Segfy.Api.Tests.Repositories;

public class ApoliceRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SegfyDbContext _context;
    private readonly ApoliceRepository _repository;

    public ApoliceRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SegfyDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SegfyDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new ApoliceRepository(_context);
    }

    [Fact]
    public async Task GetVencendoEm30DiasAsync_DeveRetornarApenasApolicesAtivasNoIntervalo()
    {
        await _repository.AddAsync(Criar("SEG-2026-0001", StatusApolice.Ativa, DateTime.Today.AddDays(10)));
        await _repository.AddAsync(Criar("SEG-2026-0002", StatusApolice.Ativa, DateTime.Today.AddDays(45)));
        await _repository.AddAsync(Criar("SEG-2026-0003", StatusApolice.Cancelada, DateTime.Today.AddDays(5)));
        await _repository.AddAsync(Criar("SEG-2026-0004", StatusApolice.Ativa, DateTime.Today.AddDays(-2)));

        var resultado = await _repository.GetVencendoEm30DiasAsync();

        Assert.Single(resultado);
        Assert.Equal("SEG-2026-0001", resultado[0].NumeroApolice);
    }

    [Fact]
    public async Task CountByYearAsync_DeveContarApenasApolicesDoAnoInformado()
    {
        await _repository.AddAsync(Criar("SEG-2025-0001", StatusApolice.Ativa, DateTime.Today.AddDays(10)));
        await _repository.AddAsync(Criar("SEG-2026-0001", StatusApolice.Ativa, DateTime.Today.AddDays(10)));
        await _repository.AddAsync(Criar("SEG-2026-0002", StatusApolice.Ativa, DateTime.Today.AddDays(10)));

        var resultado = await _repository.CountByYearAsync(2026);

        Assert.Equal(2, resultado);
    }

    [Fact]
    public async Task DeleteAsync_DeveRemoverApoliceDoBanco()
    {
        var apolice = await _repository.AddAsync(Criar("SEG-2026-0001", StatusApolice.Ativa, DateTime.Today.AddDays(10)));

        await _repository.DeleteAsync(apolice);
        var resultado = await _repository.GetByIdAsync(apolice.Id);

        Assert.Null(resultado);
    }

    private static Apolice Criar(string numero, StatusApolice status, DateTime dataFim)
    {
        return new Apolice
        {
            NumeroApolice = numero,
            CpfCnpjSegurado = "12345678900",
            PlacaVeiculo = "ABC1234",
            ValorPremio = 100,
            DataInicioVigencia = DateTime.Today,
            DataFimVigencia = dataFim,
            Status = status
        };
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
