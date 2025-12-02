using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelBackend.Domain.Entities;
using SentinelBackend.Domain.Ports;
using System.Security.Claims;
using SentinelBackend.Application.DTOs;

namespace SentinelBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetasController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public MetasController(IUnitOfWork uow) => _uow = uow;

    private string Rol => User.FindFirst(ClaimTypes.Role)?.Value!;
    private Guid UsuarioDbId
    {
        get
        {
            var claim = User.FindFirst("db_user_id")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }

    [HttpGet("mis-metas")]
    public async Task<IActionResult> GetMisMetas()
    {
        var metas = await _uow.Metas.FindAsync(m => m.UsuarioId == UsuarioDbId);
        return Ok(metas);
    }

    [HttpGet("subordinados")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> GetMetasSubordinados()
    {
        var subordinadosIds = await _uow.Usuarios
            .FindAsync(u => u.SupervisorId == UsuarioDbId)
            .ContinueWith(t => t.Result.Select(u => u.Id));

        var metas = await _uow.Metas.FindAsync(m => subordinadosIds.Contains(m.UsuarioId));
        return Ok(metas);
    }

    [HttpPost]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Crear([FromBody] CrearMetaDto dto)
    {
        if (Rol != "supervisor" && dto.UsuarioId != UsuarioDbId)
            return Forbid();

        var meta = new Metas
        {
            Id = Guid.NewGuid(),
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            PuntosObjetivo = dto.PuntosObjetivo,
            PuntosActuales = 0,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Activa = true,
            UsuarioId = dto.UsuarioId,
            Created_At = DateTime.UtcNow
        };

        await _uow.Metas.AddAsync(meta);
        await _uow.SaveChangesAsync();

        return CreatedAtAction("GetMetaById", new { id = meta.Id }, meta);
    }

    [HttpPut("{id}/completar")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Completar(Guid id)
    {
        var meta = await _uow.Metas.GetByIdAsync(id);
        if (meta == null) return NotFound();

        if (Rol != "supervisor" && meta.UsuarioId != UsuarioDbId)
            return Forbid();
        if (!meta.Activa)
            return BadRequest("La meta ya está completada");

        meta.Activa = false;
        meta.PuntosActuales = meta.PuntosObjetivo;
        
        _uow.Metas.Update(meta);
        await _uow.SaveChangesAsync();

        return Ok(meta);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMetaById(Guid id)
    {
        var meta = await _uow.Metas.GetByIdAsync(id);
        if (meta == null) return NotFound();
        return Ok(meta);
    }
}