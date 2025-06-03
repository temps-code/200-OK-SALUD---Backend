using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using _200_OK_SALUD___Backend.Data;
using _200_OK_SALUD___Backend.Models;
using _200_OK_SALUD___Backend.ViewModels;

namespace _200_OK_SALUD___Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class EspecialidadController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public EspecialidadController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Especialidad/all
        [HttpGet("all")]
        public async Task<IActionResult> ListarTodo()
        {
            var lista = await _dbContext.Especialidades
                .Select(e => new EspecialidadDto
                {
                    EspecialidadId = e.EspecialidadId,
                    Nombre = e.Nombre,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado completo obtenido exitosamente.", data = lista });
        }

        // GET: api/Especialidad/active
        [HttpGet("active")]
        public async Task<IActionResult> ListarActivos()
        {
            var lista = await _dbContext.Especialidades
                .Where(e => e.IsActive)
                .Select(e => new EspecialidadDto
                {
                    EspecialidadId = e.EspecialidadId,
                    Nombre = e.Nombre,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de especialidades activas obtenido exitosamente.", data = lista });
        }

        // GET: api/Especialidad/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> ListarInactivos()
        {
            var lista = await _dbContext.Especialidades
                .Where(e => !e.IsActive)
                .Select(e => new EspecialidadDto
                {
                    EspecialidadId = e.EspecialidadId,
                    Nombre = e.Nombre,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de especialidades inactivas obtenido exitosamente.", data = lista });
        }

        // GET: api/Especialidad/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var entidad = await _dbContext.Especialidades
                .FirstOrDefaultAsync(e => e.EspecialidadId == id);

            if (entidad == null)
                return NotFound(new { message = "Especialidad no encontrada." });

            var dto = new EspecialidadDto
            {
                EspecialidadId = entidad.EspecialidadId,
                Nombre = entidad.Nombre,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return Ok(new { message = "Especialidad obtenida exitosamente.", data = dto });
        }

        // POST: api/Especialidad
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Crear([FromForm] CrearActualizarEspecialidadDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var existe = await _dbContext.Especialidades
                .AnyAsync(e => e.Nombre.ToLower() == model.Nombre.ToLower() && e.IsActive);
            if (existe)
                return Conflict(new { message = "Ya existe una especialidad activa con ese nombre." });

            var entidad = new Especialidad
            {
                Nombre = model.Nombre
            };
            _dbContext.Especialidades.Add(entidad);
            await _dbContext.SaveChangesAsync();

            var dto = new EspecialidadDto
            {
                EspecialidadId = entidad.EspecialidadId,
                Nombre = entidad.Nombre,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return CreatedAtAction(nameof(Obtener), new { id = dto.EspecialidadId }, new { message = "Especialidad creada exitosamente.", data = dto });
        }

        // PUT: api/Especialidad/5
        [HttpPut("{id:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] CrearActualizarEspecialidadDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var entidad = await _dbContext.Especialidades
                .Where(e => e.IsActive && e.EspecialidadId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Especialidad no encontrada o inactiva." });

            // Verificar si hubo cambios
            if (entidad.Nombre == model.Nombre)
                return BadRequest(new { message = "No se detectaron cambios en los datos." });

            var existeOtro = await _dbContext.Especialidades
                .AnyAsync(e => e.Nombre.ToLower() == model.Nombre.ToLower() && e.EspecialidadId != id && e.IsActive);
            if (existeOtro)
                return Conflict(new { message = "Ya existe otra especialidad activa con ese nombre." });

            entidad.Nombre = model.Nombre;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Especialidad actualizada exitosamente." });
        }

        // DELETE lógico: api/Especialidad/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var entidad = await _dbContext.Especialidades
                .Where(e => e.IsActive && e.EspecialidadId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Especialidad no encontrada o ya inactiva." });

            entidad.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Especialidad desactivada exitosamente." });
        }

        // DELETE físico: api/Especialidad/5/fisico
        [HttpDelete("{id:int}/fisico")]
        public async Task<IActionResult> EliminarFisico(int id)
        {
            var entidad = await _dbContext.Especialidades
                .FirstOrDefaultAsync(e => e.EspecialidadId == id);
            if (entidad == null)
                return NotFound(new { message = "Especialidad no encontrada." });

            _dbContext.Especialidades.Remove(entidad);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Especialidad eliminada físicamente." });
        }

        // PUT: api/Especialidad/5/reactivar
        [HttpPut("{id:int}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var entidad = await _dbContext.Especialidades
                .Where(e => !e.IsActive && e.EspecialidadId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Especialidad no encontrada o ya activa." });

            entidad.IsActive = true;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Especialidad reactivada exitosamente." });
        }
    }
}
