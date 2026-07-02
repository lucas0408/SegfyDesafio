using Segfy.Api.Dtos;

namespace Segfy.Api.Services;

public interface IApoliceService
{
    Task<ApoliceResponseDto> CriarAsync(ApoliceCreateDto dto);
    Task<ApoliceResponseDto?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<ApoliceResponseDto>> ListarAsync();
    Task<ApoliceResponseDto?> AtualizarAsync(int id, ApoliceUpdateDto dto);
    Task<bool> RemoverAsync(int id);
    Task<IReadOnlyList<ApoliceResponseDto>> ListarVencendoEm30DiasAsync();
}
