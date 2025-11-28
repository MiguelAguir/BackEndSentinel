using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using SentinelBackend.Infrastructure.Persistence;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var uid = context.User.FindFirst("user_id")?.Value;
            if (uid != null)
            {
                var usuario = await db.Usuarios
                    .FirstOrDefaultAsync(u => u.FirebaseUid == uid);

                if (usuario != null)
                {
                    context.Items["UsuarioActual"] = usuario;
                }
            }
        }

        await _next(context);
    }
}