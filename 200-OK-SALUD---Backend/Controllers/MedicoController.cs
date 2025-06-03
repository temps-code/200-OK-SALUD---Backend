using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class MedicoController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MedicoController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Medico/all
        [HttpGet("all")]
        public async Task<IActionResult> ListarTodo()
        {
            var lista = await _dbContext.Medicos
                .Select(m => new MedicoDto
                {
                    MedicoId = m.MedicoId,
                    UsuarioId = m.UsuarioId,
                    Nombre = m.Nombre,
                    Apellido = m.Apellido,
                    Telefono = m.Telefono,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado completo de médicos obtenido correctamente.", data = lista });
        }

        // GET: api/Medico/active
        [HttpGet("active")]
        public async Task<IActionResult> ListarActivos()
        {
            var lista = await _dbContext.Medicos
                .Where(m => m.IsActive)
                .Select(m => new MedicoDto
                {
                    MedicoId = m.MedicoId,
                    UsuarioId = m.UsuarioId,
                    Nombre = m.Nombre,
                    Apellido = m.Apellido,
                    Telefono = m.Telefono,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de médicos activos obtenido correctamente.", data = lista });
        }

        // GET: api/Medico/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> ListarInactivos()
        {
            var lista = await _dbContext.Medicos
                .Where(m => !m.IsActive)
                .Select(m => new MedicoDto
                {
                    MedicoId = m.MedicoId,
                    UsuarioId = m.UsuarioId,
                    Nombre = m.Nombre,
                    Apellido = m.Apellido,
                    Telefono = m.Telefono,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de médicos inactivos obtenido correctamente.", data = lista });
        }

        // GET: api/Medico/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var entidad = await _dbContext.Medicos
                .FirstOrDefaultAsync(m => m.MedicoId == id);

            if (entidad == null)
                return NotFound(new { message = "Médico no encontrado." });

            var dto = new MedicoDto
            {
                MedicoId = entidad.MedicoId,
                UsuarioId = entidad.UsuarioId,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                Telefono = entidad.Telefono,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return Ok(new { message = "Médico obtenido correctamente.", data = dto });
        }

        // POST: api/Medico
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Crear([FromForm] CrearMedicoDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Datos inválidos.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            // 1. Verificar que el email no exista en AspNetUsers
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return Conflict(new { message = "Ya existe un usuario con ese email." });

            // 2. Crear la cuenta de Identity para el médico
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Error al crear la cuenta de usuario.",
                    errors = createUserResult.Errors.Select(e => e.Description)
                });
            }

            // 3. Asegurar que exista el rol "Medico"
            if (!await _roleManager.RoleExistsAsync("Medico"))
                await _roleManager.CreateAsync(new IdentityRole("Medico"));

            // 4. Asignar rol "Medico" al usuario
            await _userManager.AddToRoleAsync(user, "Medico");

            // 5. Verificar duplicidad de datos en Medico
            var dup = await _dbContext.Medicos.AnyAsync(m =>
                m.Nombre.ToLower() == model.Nombre.ToLower() &&
                m.Apellido.ToLower() == model.Apellido.ToLower() &&
                m.Telefono == model.Telefono &&
                m.IsActive);
            if (dup)
            {
                // Deshacer creación de usuario si ya hay duplicidad
                await _userManager.DeleteAsync(user);
                return Conflict(new { message = "Ya existe un médico activo con los mismos datos." });
            }

            // 6. Crear el registro en Medicos usando UsuarioId = user.Id
            var entidad = new Medico
            {
                UsuarioId = user.Id,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Telefono = model.Telefono
            };

            _dbContext.Medicos.Add(entidad);
            await _dbContext.SaveChangesAsync();

            var dto = new MedicoDto
            {
                MedicoId = entidad.MedicoId,
                UsuarioId = entidad.UsuarioId,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                Telefono = entidad.Telefono,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return CreatedAtAction(nameof(Obtener), new { id = dto.MedicoId }, new { message = "Médico creado exitosamente.", data = dto });
        }

        // PUT: api/Medico/5
        [HttpPut("{id:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] ActualizarMedicoDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Datos inválidos.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            var entidad = await _dbContext.Medicos
                .Where(m => m.IsActive && m.MedicoId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Médico no encontrado o inactivo." });

            // Verificar si hubo cambios
            bool cambio =
                entidad.Nombre != model.Nombre ||
                entidad.Apellido != model.Apellido ||
                entidad.Telefono != model.Telefono;

            if (!cambio)
                return BadRequest(new { message = "No se detectaron cambios en los datos." });

            // Verificar duplicidad con otro médico activo
            var dup = await _dbContext.Medicos
                .AnyAsync(m =>
                    m.MedicoId != id &&
                    m.Nombre.ToLower() == model.Nombre.ToLower() &&
                    m.Apellido.ToLower() == model.Apellido.ToLower() &&
                    m.Telefono == model.Telefono &&
                    m.IsActive);
            if (dup)
                return Conflict(new { message = "Ya existe otro médico activo con los mismos datos." });

            entidad.Nombre = model.Nombre;
            entidad.Apellido = model.Apellido;
            entidad.Telefono = model.Telefono;

            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Médico actualizado exitosamente." });
        }

        // DELETE lógico: api/Medico/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var entidad = await _dbContext.Medicos
                .Where(m => m.IsActive && m.MedicoId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Médico no encontrado o ya inactivo." });

            entidad.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Médico desactivado exitosamente." });
        }

        // DELETE físico: api/Medico/5/fisico
        [HttpDelete("{id:int}/fisico")]
        public async Task<IActionResult> EliminarFisico(int id)
        {
            var entidad = await _dbContext.Medicos
                .FirstOrDefaultAsync(m => m.MedicoId == id);
            if (entidad == null)
                return NotFound(new { message = "Médico no encontrado." });

            _dbContext.Medicos.Remove(entidad);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Médico eliminado físicamente." });
        }

        // PUT: api/Medico/5/reactivar
        [HttpPut("{id:int}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var entidad = await _dbContext.Medicos
                .Where(m => !m.IsActive && m.MedicoId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Médico no encontrado o ya activo." });

            entidad.IsActive = true;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Médico reactivado exitosamente." });
        }
    }
}
