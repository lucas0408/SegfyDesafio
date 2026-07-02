using Microsoft.AspNetCore.Mvc;
using Segfy.Api.Dtos;
using Segfy.Api.Services;

namespace Segfy.Api.Controllers;

[ApiController]
[Route("api/apolices")]
public class ApolicesController : ControllerBase
{
    private readonly IApoliceService _service;

    public ApolicesController(IApoliceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApoliceResponseDto>>> ListarTodas()
    {
        var apolices = await _service.ListarAsync();
        return Ok(apolices);
    }

    [HttpGet("vencendo-em-30-dias")]
    public async Task<ActionResult<IReadOnlyList<ApoliceResponseDto>>> ListarVencendoEm30Dias()
    {
        var apolices = await _service.ListarVencendoEm30DiasAsync();
        return Ok(apolices);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApoliceResponseDto>> ObterPorId(int id)
    {
        var apolice = await _service.ObterPorIdAsync(id);
        if (apolice is null)
        {
            return NotFound();
        }

        return Ok(apolice);
    }

    [HttpPost]
    public async Task<ActionResult<ApoliceResponseDto>> Criar([FromBody] ApoliceCreateDto dto)
    {
        var apolice = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = apolice.Id }, apolice);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApoliceResponseDto>> Atualizar(int id, [FromBody] ApoliceUpdateDto dto)
    {
        var apolice = await _service.AtualizarAsync(id, dto);
        if (apolice is null)
        {
            return NotFound();
        }

        return Ok(apolice);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        var removido = await _service.RemoverAsync(id);
        if (!removido)
        {
            return NotFound();
        }

        return NoContent();
    }
}
