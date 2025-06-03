using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using _200_OK_SALUD___Backend.Models;  // Ajusta el namespace según tu proyecto

namespace _200_OK_SALUD___Backend.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            // 1. Obtener RoleManager y UserManager del contenedor de servicios
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 2. Definir los nombres de los roles que queremos crear
            string[] roles = new[] { "Administrador", "Medico", "Paciente" };

            // 3. Crear cada rol si no existe
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 4. Crear usuario administrador inicial si no existe
            var adminEmail = "admin@clinica.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                // Cambia “Admin123!” por la contraseña que desees (mínimo 6 caracteres y cumpla la política)
                var result = await userManager.CreateAsync(newAdmin, "Admin123!");
                if (result.Succeeded)
                {
                    // Asignar rol “Administrador” al nuevo usuario
                    await userManager.AddToRoleAsync(newAdmin, "Administrador");
                }
            }

            // 5. Crear usuario medico inicial si no existe
            var medicoEmail = "medico@clinica.com";
            var existMedico = await userManager.FindByEmailAsync(medicoEmail);
            if (existMedico == null)
            {
                var newMedico = new ApplicationUser
                {
                    UserName = medicoEmail,
                    Email = medicoEmail,
                    EmailConfirmed = true
                };
                var resultMedico = await userManager.CreateAsync(newMedico, "Medico123!");
                if (resultMedico.Succeeded)
                {
                    await userManager.AddToRoleAsync(newMedico, "Medico");
                }
            }
        }
    }
}
