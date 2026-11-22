
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace AutoRentExpress.Security
{
    public class PermisosPorRol : IControllerModelConvention
    {
        private static readonly HashSet<string> ModulosAdministrador =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "Tarifas",
                "Seguros",
                "Empleados",
                "Sucursales",
                "Marcas",
                "Modelos",
                "CategoriaVehiculo",
                "EstadosVehiculo"
            };

        private static readonly Dictionary<string, HashSet<string>>
            AccionesPermitidasEmpleado =
                new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Clientes"] = new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase)
                    {
                        "Index",
                        "Create",
                        "Details",
                        "Detalles"
                    },

                    ["Vehiculos"] = new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase)
                    {
                        "Index",
                        "Details",
                        "Detalles"
                    },

                    ["Reservas"] = new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase)
                    {
                        "Index",
                        "Disponibles",
                        "Create",
                        "Details",
                        "Detalles"
                    }
                };

        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerName.Equals(
                "Cuenta",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (ModulosAdministrador.Contains(controller.ControllerName))
            {
                controller.Filters.Add(
                    CrearFiltroAdministrador());

                return;
            }

            if (!AccionesPermitidasEmpleado.TryGetValue(
                controller.ControllerName,
                out var accionesPermitidas))
            {
                return;
            }

            foreach (var accion in controller.Actions)
            {
                if (!accionesPermitidas.Contains(accion.ActionName))
                {
                    accion.Filters.Add(
                        CrearFiltroAdministrador());
                }
            }
        }

        private static AuthorizeFilter CrearFiltroAdministrador()
        {
            var politica = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Administrador")
                .Build();

            return new AuthorizeFilter(politica);
        }
    }
}
