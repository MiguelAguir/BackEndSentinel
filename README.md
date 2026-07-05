# BackEndSentinel

Backend del sistema Sentinel, una API REST construida con ASP.NET Core y Clean Architecture para la gestión de productividad con gamificación.

## Descripción

BackEndSentinel es una API que permite gestionar usuarios, tareas, metas, evidencias, logros y recompensas en un entorno laboral con roles de supervisor y usuario. Incluye autenticación JWT validada con Firebase, generación de reportes en Excel, y un sistema de puntos y recompensas para motivar la productividad del equipo.

## Tecnologías

- **.NET 9** / ASP.NET Core Web API
- **Clean Architecture** (Domain, Application, Infrastructure, Api)
- **Entity Framework Core 9** con PostgreSQL (Supabase)
- **Autenticación JWT + Firebase Admin SDK**
- **ClosedXML / QuestPDF** para generación de reportes
- **Serilog** para logging estructurado
- **FluentValidation** para validación de DTOs
- **Swagger / OpenAPI** para documentación de endpoints

## Estructura del proyecto

```
BackEndSentinel/
  SentinelBackend.Api/           -- Capa de presentación (controllers, middleware)
  SentinelBackend.Application/   -- DTOs y lógica de aplicación
  SentinelBackend.Domain/        -- Entidades, puertos (repositorios, unit of work)
  SentinelBackend.Infrastructure/-- Persistencia (DbContext, repositorios, servicios)
```

### Entidades del dominio

| Entidad       | Descripción                                      |
|---------------|--------------------------------------------------|
| Usuarios      | Usuarios con rol (usuario/supervisor) y jerarquía |
| Tareas        | Tareas asignadas con puntos, estado y evidencias |
| Metas         | Objetivos con puntaje objetivo para los usuarios |
| Evidencias    | Soporte (foto/texto/archivo) para aprobar tareas |
| Logros        | Registro de puntos ganados por tarea             |
| Recompensas   | Premios canjeables con puntos                    |
| Gamificacion  | Puntaje total acumulado por usuario              |
| Perfiles      | Atributos adicionales del usuario (JSON)         |

## Instalación

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/MiguelAguir/BackEndSentinel.git
   cd BackEndSentinel
   ```

2. Configurar la cadena de conexión en `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Supabase": "Host=...;Port=6543;Database=postgres;Username=...;Password=...;Ssl Mode=Require;"
     }
   }
   ```

3. Agregar el archivo `firebase-service-account.json` con las credenciales de Firebase.

4. Aplicar migraciones:
   ```bash
   dotnet ef database update
   ```

5. Ejecutar:
   ```bash
   dotnet run --project SentinelBackend.Api
   ```

## Endpoints

### Usuarios
| Método | Ruta                                | Roles        | Descripción                    |
|--------|-------------------------------------|--------------|--------------------------------|
| POST   | /api/usuarios/registrar-subordinado | supervisor   | Registrar nuevo usuario        |
| GET    | /api/usuarios/mis-subordinados      | supervisor   | Listar subordinados            |
| GET    | /api/usuarios/{id}                  | autenticado  | Obtener usuario por ID         |

### Tareas
| Método | Ruta                    | Roles        | Descripción                     |
|--------|-------------------------|--------------|---------------------------------|
| GET    | /api/tareas/mis-tareas  | autenticado  | Listar mis tareas               |
| GET    | /api/tareas/subordinados| supervisor   | Tareas de subordinados          |
| POST   | /api/tareas             | supervisor   | Crear tarea                     |
| PUT    | /api/tareas/{id}        | supervisor   | Actualizar tarea                |
| DELETE | /api/tareas/{id}        | supervisor   | Eliminar tarea                  |

### Metas
| Método | Ruta                        | Roles        | Descripción              |
|--------|-----------------------------|--------------|--------------------------|
| GET    | /api/metas/mis-metas        | autenticado  | Listar mis metas         |
| GET    | /api/metas/subordinados     | supervisor   | Metas de subordinados    |
| POST   | /api/metas                  | supervisor   | Crear meta               |
| PUT    | /api/metas/{id}/completar   | supervisor   | Completar meta           |

### Reportes
| Método | Ruta                      | Roles        | Descripción                        |
|--------|---------------------------|--------------|------------------------------------|
| GET    | /api/reportes/mi-reporte  | autenticado  | Descargar reporte personal (Excel) |
| GET    | /api/reportes/equipo      | supervisor   | Descargar reporte de equipo (Excel)|

## Estado del proyecto

En desarrollo activo. La funcionalidad base de autenticación, CRUD de tareas/usuarios/metas y generación de reportes está operativa.
