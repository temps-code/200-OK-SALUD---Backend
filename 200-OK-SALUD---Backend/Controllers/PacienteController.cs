using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PacienteController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Paciente/all
        [HttpGet("all")]
        public async Task<IActionResult> ListarTodo()
        {
            var lista = await _dbContext.Pacientes
                .Select(p => new PacienteDto
                {
                    PacienteId = p.PacienteId,
                    UsuarioId = p.UsuarioId,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    FechaNacimiento = p.FechaNacimiento,
                    Genero = p.Genero,
                    Telefono = p.Telefono,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado completo de pacientes obtenido correctamente.", data = lista });
        }

        // GET: api/Paciente/active
        [HttpGet("active")]
        public async Task<IActionResult> ListarActivos()
        {
            var lista = await _dbContext.Pacientes
                .Where(p => p.IsActive)
                .Select(p => new PacienteDto
                {
                    PacienteId = p.PacienteId,
                    UsuarioId = p.UsuarioId,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    FechaNacimiento = p.FechaNacimiento,
                    Genero = p.Genero,
                    Telefono = p.Telefono,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de pacientes activos obtenido correctamente.", data = lista });
        }

        // GET: api/Paciente/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> ListarInactivos()
        {
            var lista = await _dbContext.Pacientes
                .Where(p => !p.IsActive)
                .Select(p => new PacienteDto
                {
                    PacienteId = p.PacienteId,
                    UsuarioId = p.UsuarioId,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    FechaNacimiento = p.FechaNacimiento,
                    Genero = p.Genero,
                    Telefono = p.Telefono,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { message = "Listado de pacientes inactivos obtenido correctamente.", data = lista });
        }

        // GET: api/Paciente/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var entidad = await _dbContext.Pacientes
                .FirstOrDefaultAsync(p => p.PacienteId == id);

            if (entidad == null)
                return NotFound(new { message = "Paciente no encontrado." });

            var dto = new PacienteDto
            {
                PacienteId = entidad.PacienteId,
                UsuarioId = entidad.UsuarioId,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                FechaNacimiento = entidad.FechaNacimiento,
                Genero = entidad.Genero,
                Telefono = entidad.Telefono,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return Ok(new { message = "Paciente obtenido correctamente.", data = dto });
        }

        // POST: api/Paciente
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Crear([FromForm] CrearPacienteDto model)
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

            // 2. Crear la cuenta de Identity para el paciente
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

            // 3. Asegurar que exista el rol "Paciente"
            if (!await _roleManager.RoleExistsAsync("Paciente"))
                await _roleManager.CreateAsync(new IdentityRole("Paciente"));

            // 4. Asignar rol "Paciente" al usuario
            await _userManager.AddToRoleAsync(user, "Paciente");

            // 5. Verificar duplicidad de datos en Paciente
            var dup = await _dbContext.Pacientes.AnyAsync(p =>
                p.Nombre.ToLower() == model.Nombre.ToLower() &&
                p.Apellido.ToLower() == model.Apellido.ToLower() &&
                p.FechaNacimiento == model.FechaNacimiento &&
                p.IsActive);
            if (dup)
            {
                // Deshacer creación de usuario si ya hay duplicidad
                await _userManager.DeleteAsync(user);
                return Conflict(new { message = "Ya existe un paciente activo con los mismos datos." });
            }

            // 6. Crear el registro en Pacientes usando UsuarioId = user.Id
            var entidad = new Paciente
            {
                UsuarioId = user.Id,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                FechaNacimiento = model.FechaNacimiento,
                Genero = model.Genero,
                Telefono = model.Telefono
            };

            _dbContext.Pacientes.Add(entidad);
            await _dbContext.SaveChangesAsync();

            var dto = new PacienteDto
            {
                PacienteId = entidad.PacienteId,
                UsuarioId = entidad.UsuarioId,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                FechaNacimiento = entidad.FechaNacimiento,
                Genero = entidad.Genero,
                Telefono = entidad.Telefono,
                IsActive = entidad.IsActive,
                CreatedAt = entidad.CreatedAt,
                UpdatedAt = entidad.UpdatedAt
            };

            return CreatedAtAction(nameof(Obtener), new { id = dto.PacienteId }, new { message = "Paciente creado exitosamente.", data = dto });
        }

        // PUT: api/Paciente/5
        [HttpPut("{id:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Actualizar(int id, [FromForm] ActualizarPacienteDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Datos inválidos.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            var entidad = await _dbContext.Pacientes
                .Where(p => p.IsActive && p.PacienteId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Paciente no encontrado o inactivo." });

            // Verificar si hubo cambios
            bool cambio =
                entidad.Nombre != model.Nombre ||
                entidad.Apellido != model.Apellido ||
                entidad.FechaNacimiento != model.FechaNacimiento ||
                entidad.Genero != model.Genero ||
                entidad.Telefono != model.Telefono;

            if (!cambio)
                return BadRequest(new { message = "No se detectaron cambios en los datos." });

            // Verificar duplicidad con otro paciente activo
            var dupPaciente = await _dbContext.Pacientes.AnyAsync(p =>
                p.PacienteId != id &&
                p.Nombre.ToLower() == model.Nombre.ToLower() &&
                p.Apellido.ToLower() == model.Apellido.ToLower() &&
                p.FechaNacimiento == model.FechaNacimiento &&
                p.IsActive);
            if (dupPaciente)
                return Conflict(new { message = "Ya existe otro paciente activo con los mismos datos." });

            entidad.Nombre = model.Nombre;
            entidad.Apellido = model.Apellido;
            entidad.FechaNacimiento = model.FechaNacimiento;
            entidad.Genero = model.Genero;
            entidad.Telefono = model.Telefono;

            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Paciente actualizado exitosamente." });
        }

        // DELETE lógico: api/Paciente/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var entidad = await _dbContext.Pacientes
                .Where(p => p.IsActive && p.PacienteId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Paciente no encontrado o ya inactivo." });

            entidad.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Paciente desactivado exitosamente." });
        }

        // DELETE físico: api/Paciente/5/fisico
        [HttpDelete("{id:int}/fisico")]
        public async Task<IActionResult> EliminarFisico(int id)
        {
            var entidad = await _dbContext.Pacientes
                .FirstOrDefaultAsync(p => p.PacienteId == id);
            if (entidad == null)
                return NotFound(new { message = "Paciente no encontrado." });

            _dbContext.Pacientes.Remove(entidad);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Paciente eliminado físicamente." });
        }

        // PUT: api/Paciente/5/reactivar
        [HttpPut("{id:int}/reactivar")]
        public async Task<IActionResult> Reactivar(int id)
        {
            var entidad = await _dbContext.Pacientes
                .Where(p => !p.IsActive && p.PacienteId == id)
                .FirstOrDefaultAsync();
            if (entidad == null)
                return NotFound(new { message = "Paciente no encontrado o ya activo." });

            entidad.IsActive = true;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Paciente reactivado exitosamente." });
        }
    }
}
