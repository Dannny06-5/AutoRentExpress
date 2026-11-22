
using Microsoft.AspNetCore.Identity;

namespace AutoRentExpress.Security
{
    public static class IdentitySeeder
    {
        public static async Task InicializarAsync(
            IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var proveedor = scope.ServiceProvider;

            var roleManager = proveedor
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = proveedor
                .GetRequiredService<UserManager<IdentityUser>>();

            var environment = proveedor
                .GetRequiredService<IHostEnvironment>();

            foreach (var nombre in new[] { "Administrador", "Empleado" })
            {
                if (!await roleManager.RoleExistsAsync(nombre))
                {
                    var resultado = await roleManager.CreateAsync(
                        new IdentityRole(nombre));

                    Verificar(resultado);
                }
            }

            if (!environment.IsDevelopment())
                return;

            var administradores = await userManager
                .GetUsersInRoleAsync("Administrador");

            var administrador = await userManager
                .FindByNameAsync("admin");

            if (administrador == null)
            {
                administrador = administradores.FirstOrDefault();
            }

            if (administrador == null)
            {
                administrador = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@autorentexpress.com",
                    EmailConfirmed = true
                };

                Verificar(await userManager.CreateAsync(
                    administrador, "1234"));
            }
            else if (administrador.UserName != "admin")
            {
                var otroAdmin = await userManager
                    .FindByNameAsync("admin");

                if (otroAdmin != null && otroAdmin.Id != administrador.Id)
                {
                    throw new InvalidOperationException(
                        "El nombre admin ya está ocupado.");
                }

                Verificar(await userManager.SetUserNameAsync(
                    administrador, "admin"));
            }

            if (!await userManager.IsInRoleAsync(
                administrador, "Administrador"))
            {
                Verificar(await userManager.AddToRoleAsync(
                    administrador, "Administrador"));
            }

            await EstablecerClaveDemoAsync(
                userManager, administrador);

            var empleado = await userManager
                .FindByNameAsync("empleado");

            if (empleado == null)
            {
                empleado = new IdentityUser
                {
                    UserName = "empleado",
                    Email = "empleado@autorentexpress.com",
                    EmailConfirmed = true
                };

                Verificar(await userManager.CreateAsync(
                    empleado, "1234"));
            }

            if (!await userManager.IsInRoleAsync(
                empleado, "Empleado"))
            {
                Verificar(await userManager.AddToRoleAsync(
                    empleado, "Empleado"));
            }

            await EstablecerClaveDemoAsync(
                userManager, empleado);
        }

        private static async Task EstablecerClaveDemoAsync(
            UserManager<IdentityUser> userManager,
            IdentityUser usuario)
        {
            if (await userManager.CheckPasswordAsync(usuario, "1234"))
                return;

            var token = await userManager
                .GeneratePasswordResetTokenAsync(usuario);

            var resultado = await userManager.ResetPasswordAsync(
                usuario,
                token,
                "1234");

            Verificar(resultado);
        }

        private static void Verificar(IdentityResult resultado)
        {
            if (resultado.Succeeded)
                return;

            var errores = string.Join(
                "; ",
                resultado.Errors.Select(e => e.Description));

            throw new InvalidOperationException(errores);
        }
    }
}
