using Segfy.Api.Models;

namespace Segfy.Api.Repositories;

public interface IApoliceRepository
{
    Task<Apolice> AddAsync(Apolice apolice);
    Task<Apolice?> GetByIdAsync(int id);
    Task<IReadOnlyList<Apolice>> GetAllAsync();
    Task UpdateAsync(Apolice apolice);
    Task DeleteAsync(Apolice apolice);
    Task<int> CountByYearAsync(int year);
    Task<IReadOnlyList<Apolice>> GetVencendoEm30DiasAsync();
}
