using Microsoft.EntityFrameworkCore;
using Segfy.Api.Data;
using Segfy.Api.Models;

namespace Segfy.Api.Repositories;

public class ApoliceRepository : IApoliceRepository
{
    private readonly SegfyDbContext _context;

    public ApoliceRepository(SegfyDbContext context)
    {
        _context = context;
    }

    public async Task<Apolice> AddAsync(Apolice apolice)
    {
        _context.Apolices.Add(apolice);
        await _context.SaveChangesAsync();
        return apolice;
    }

    public async Task<Apolice?> GetByIdAsync(int id)
    {
        return await _context.Apolices.FindAsync(id);
    }

    public async Task<IReadOnlyList<Apolice>> GetAllAsync()
    {
        return await _context.Apolices
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    public async Task UpdateAsync(Apolice apolice)
    {
        _context.Apolices.Update(apolice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Apolice apolice)
    {
        _context.Apolices.Remove(apolice);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountByYearAsync(int year)
    {
        return await _context.Apolices
            .CountAsync(a => a.NumeroApolice.StartsWith($"SEG-{year}-"));
    }

    public async Task<IReadOnlyList<Apolice>> GetVencendoEm30DiasAsync()
    {
        var hoje = DateTime.Today;
        var limite = hoje.AddDays(30);
        var statusAtiva = StatusApolice.Ativa.ToString();

        return await _context.Apolices
            .FromSqlInterpolated($@"
                SELECT * FROM Apolices
                WHERE Status = {statusAtiva}
                  AND date(DataFimVigencia) BETWEEN date({hoje}) AND date({limite})
                ORDER BY DataFimVigencia ASC")
            .ToListAsync();
    }
}
