// Controllers/ReportesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelBackend.Api.Services;
using SentinelBackend.Domain.Ports;

namespace SentinelBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // obligatorio token
public class ReportesController : ControllerBase
{
    private readonly ReporteService _reporteService;
    private readonly IUnitOfWork _uow;

    public ReportesController(ReporteService reporteService, IUnitOfWork uow)
    {
        _reporteService = reporteService;
        _uow = uow;
    }

    private Guid UsuarioId => 
        Guid.TryParse(User.FindFirst("db_user_id")?.Value, out var id) ? id : Guid.Empty;

    // MI REPORTE PERSONAL
    [HttpGet("mi-reporte")]
    public async Task<IActionResult> MiReporte()
    {
        if (UsuarioId == Guid.Empty) return Unauthorized();

        var excel = await _reporteService.GenerarReportePersonalAsync(UsuarioId);
        return File(excel, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Mi_Reporte_{DateTime.Now:yyyy-MM-dd}.xlsx");
    }

    // REPORTE DE EQUIPO (solo supervisor)
    [HttpGet("equipo")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> ReporteEquipo()
    {
        if (UsuarioId == Guid.Empty) return Unauthorized();

        var excel = await _reporteService.GenerarReporteEquipoAsync(UsuarioId);
        return File(excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reporte_Equipo_{DateTime.Now:yyyy-MM-dd_HHmm}.xlsx");
    }
}