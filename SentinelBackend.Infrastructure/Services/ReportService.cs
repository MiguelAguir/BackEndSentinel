// Services/ReporteService.cs
using ClosedXML.Excel;
using SentinelBackend.Domain.Ports;

namespace SentinelBackend.Api.Services;

public class ReporteService
{
    private readonly IUnitOfWork _uow;

    public ReporteService(IUnitOfWork uow) => _uow = uow;

    public async Task<byte[]> GenerarReportePersonalAsync(Guid usuarioId)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null) throw new Exception("Usuario no encontrado");

        var tareas = await _uow.Tareas.FindAsync(t => t.UsuarioId == usuarioId);
        var metas = await _uow.Metas.FindAsync(m => m.UsuarioId == usuarioId);

        var puntosTotales = tareas
            .Where(t => t.Estado == "completada")
            .Sum(t => t.Puntos);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Reporte");

        ws.Cell(1, 1).Value = "REPORTE PERSONAL";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 16;
        ws.Range(1, 1, 1, 6).Merge();

        ws.Cell(2, 1).Value = "Nombre:";
        ws.Cell(2, 2).Value = usuario.Nombre;

        ws.Cell(3, 1).Value = "Email:";
        ws.Cell(3, 2).Value = usuario.Email;

        ws.Cell(4, 1).Value = "Rol:";
        ws.Cell(4, 2).Value = usuario.Rol;

        ws.Cell(5, 1).Value = "Puntos Totales:";
        ws.Cell(5, 2).Value = puntosTotales;

        ws.Cell(7, 1).Value = "Tareas Totales:";
        ws.Cell(7, 2).Value = tareas.Count();

        ws.Cell(8, 1).Value = "Tareas Completadas:";
        ws.Cell(8, 2).Value = tareas.Count(t => t.Estado == "completada");

        ws.Cell(9, 1).Value = "Metas Activas:";
        ws.Cell(9, 2).Value = metas.Count(m => m.Activa);

        ws.Cell(10, 1).Value = "Metas Completadas:";
        ws.Cell(10, 2).Value = metas.Count(m => m.PuntosActuales >= m.PuntosObjetivo);

        // Lista simple de tareas
        ws.Cell(12, 1).Value = "TAREAS";
        ws.Cell(12, 1).Style.Font.Bold = true;

        int fila = 13;
        foreach (var t in tareas)
        {
            ws.Cell(fila, 1).Value = t.Titulo;
            ws.Cell(fila, 2).Value = t.Descripcion ?? "";
            ws.Cell(fila, 3).Value = t.Estado;
            ws.Cell(fila, 4).Value = t.Puntos;
            fila++;
        }

        // Lista simple de metas
        ws.Cell(fila + 2, 1).Value = "METAS";
        ws.Cell(fila + 2, 1).Style.Font.Bold = true;

        fila += 3;
        foreach (var m in metas)
        {
            ws.Cell(fila, 1).Value = m.Titulo;
            ws.Cell(fila, 2).Value = m.PuntosObjetivo;
            ws.Cell(fila, 3).Value = m.PuntosActuales;
            ws.Cell(fila, 4).Value = m.Activa ? "Activa" : "Inactiva";
            fila++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerarReporteEquipoAsync(Guid supervisorId)
    {
        var subordinados = await _uow.Usuarios.FindAsync(u => u.SupervisorId == supervisorId);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Equipo");

        ws.Cell(1, 1).Value = "REPORTE DE EQUIPO";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 16;
        ws.Range(1, 1, 1, 6).Merge();

        int fila = 3;
        ws.Cell(fila, 1).Value = "Nombre";
        ws.Cell(fila, 2).Value = "Email";
        ws.Cell(fila, 3).Value = "Tareas";
        ws.Cell(fila, 4).Value = "Completadas";
        ws.Cell(fila, 5).Value = "Puntos";
        fila++;

        foreach (var sub in subordinados)
        {
            var tareasSub = await _uow.Tareas.FindAsync(t => t.UsuarioId == sub.Id);
            int completadas = tareasSub.Count(t => t.Estado == "completada");
            int puntos = tareasSub.Where(t => t.Estado == "completada").Sum(t => t.Puntos);

            ws.Cell(fila, 1).Value = sub.Nombre;
            ws.Cell(fila, 2).Value = sub.Email;
            ws.Cell(fila, 3).Value = tareasSub.Count();
            ws.Cell(fila, 4).Value = completadas;
            ws.Cell(fila, 5).Value = puntos;
            fila++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        return stream.ToArray();
    }
}