using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<LicenciaConducir> LicenciasConducir => Set<LicenciaConducir>();
        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Empleado> Empleados => Set<Empleado>();
        public DbSet<CategoriaVehiculo> CategoriaVehiculo => Set<CategoriaVehiculo>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Modelo> Modelos => Set<Modelo>();
        public DbSet<EstadoVehiculo> EstadosVehiculo => Set<EstadoVehiculo>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
        public DbSet<Tarifa> Tarifas => Set<Tarifa>();
        public DbSet<Reserva> Reservas => Set<Reserva>();
        public DbSet<Contrato> Contratos => Set<Contrato>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Seguro> Seguros => Set<Seguro>();
        public DbSet<Multa> Multas => Set<Multa>();
        public DbSet<Mantenimiento> Mantenimientos => Set<Mantenimiento>();
        public DbSet<Inspeccion> Inspecciones => Set<Inspeccion>();
        public DbSet<ReservaDetalle> ReservaDetalles => Set<ReservaDetalle>();
        public DbSet<ContratoDetalle> ContratoDetalles => Set<ContratoDetalle>();
        public DbSet<MantenimientoDetalle> MantenimientoDetalles => Set<MantenimientoDetalle>();
        public DbSet<InspeccionDetalle> InspeccionDetalles => Set<InspeccionDetalle>();
        public DbSet<PagoDetalle> PagoDetalles => Set<PagoDetalle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoriaVehiculo>().ToTable("CategoriasVehiculo");
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasIndex(x => x.Identificacion)
                .IsUnique();

            modelBuilder.Entity<LicenciaConducir>()
                .HasIndex(x => x.Numero)
                .IsUnique();

            modelBuilder.Entity<Marca>()
                .HasIndex(x => x.Nombre)
                .IsUnique();

            modelBuilder.Entity<Modelo>()
                .HasIndex(x => new { x.MarcaId, x.Nombre })
                .IsUnique();

            modelBuilder.Entity<EstadoVehiculo>()
                .HasIndex(x => x.Nombre)
                .IsUnique();

            modelBuilder.Entity<Vehiculo>()
                .HasIndex(x => x.Placa)
                .IsUnique();

            modelBuilder.Entity<Contrato>()
                .HasIndex(x => x.NumeroContrato)
                .IsUnique();

            modelBuilder.Entity<LicenciaConducir>()
                .HasOne(x => x.Cliente)
                .WithMany(x => x.Licencias)
                .HasForeignKey(x => x.ClienteId);

            modelBuilder.Entity<Empleado>()
                .HasOne(x => x.Sucursal)
                .WithMany(x => x.Empleados)
                .HasForeignKey(x => x.SucursalId);

            modelBuilder.Entity<Modelo>()
                .HasOne(x => x.Marca)
                .WithMany(x => x.Modelos)
                .HasForeignKey(x => x.MarcaId);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(x => x.Modelo)
                .WithMany(x => x.Vehiculos)
                .HasForeignKey(x => x.ModeloId);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(x => x.CategoriaVehiculo)
                .WithMany(x => x.Vehiculos)
                .HasForeignKey(x => x.CategoriaVehiculoId);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(x => x.EstadoVehiculo)
                .WithMany(x => x.Vehiculos)
                .HasForeignKey(x => x.EstadoVehiculoId);

            modelBuilder.Entity<Vehiculo>()
                .HasOne(x => x.Sucursal)
                .WithMany(x => x.Vehiculos)
                .HasForeignKey(x => x.SucursalId);

            modelBuilder.Entity<Tarifa>()
                .HasOne(x => x.CategoriaVehiculo)
                .WithMany(x => x.Tarifas)
                .HasForeignKey(x => x.CategoriaVehiculoId);

            modelBuilder.Entity<Reserva>()
                .HasOne(x => x.Cliente)
                .WithMany(x => x.Reservas)
                .HasForeignKey(x => x.ClienteId);

            modelBuilder.Entity<Reserva>()
                .HasOne(x => x.Vehiculo)
                .WithMany(x => x.Reservas)
                .HasForeignKey(x => x.VehiculoId);

            modelBuilder.Entity<Reserva>()
                .HasOne(x => x.SucursalRetiro)
                .WithMany()
                .HasForeignKey(x => x.SucursalRetiroId);

            modelBuilder.Entity<Reserva>()
                .HasOne(x => x.SucursalDevolucion)
                .WithMany()
                .HasForeignKey(x => x.SucursalDevolucionId);

            modelBuilder.Entity<Contrato>()
                .HasOne(x => x.Reserva)
                .WithOne(x => x.Contrato)
                .HasForeignKey<Contrato>(x => x.ReservaId);

            modelBuilder.Entity<Contrato>()
                .HasOne(x => x.Empleado)
                .WithMany(x => x.Contratos)
                .HasForeignKey(x => x.EmpleadoId);

            modelBuilder.Entity<Pago>()
                .HasOne(x => x.Contrato)
                .WithMany(x => x.Pagos)
                .HasForeignKey(x => x.ContratoId);

            modelBuilder.Entity<Multa>()
                .HasOne(x => x.Contrato)
                .WithMany(x => x.Multas)
                .HasForeignKey(x => x.ContratoId);

            modelBuilder.Entity<Mantenimiento>()
                .HasOne(x => x.Vehiculo)
                .WithMany(x => x.Mantenimientos)
                .HasForeignKey(x => x.VehiculoId);

            modelBuilder.Entity<Mantenimiento>()
                .HasOne(x => x.Empleado)
                .WithMany(x => x.Mantenimientos)
                .HasForeignKey(x => x.EmpleadoId);

            modelBuilder.Entity<Inspeccion>()
                .HasOne(x => x.Contrato)
                .WithMany(x => x.Inspecciones)
                .HasForeignKey(x => x.ContratoId);

            modelBuilder.Entity<Inspeccion>()
                .HasOne(x => x.Empleado)
                .WithMany(x => x.Inspecciones)
                .HasForeignKey(x => x.EmpleadoId);

            modelBuilder.Entity<ReservaDetalle>()
                .HasOne(x => x.Reserva)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.ReservaId);

            modelBuilder.Entity<ReservaDetalle>()
                .HasOne(x => x.Seguro)
                .WithMany(x => x.ReservaDetalles)
                .HasForeignKey(x => x.SeguroId);

            modelBuilder.Entity<ContratoDetalle>()
                .HasOne(x => x.Contrato)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.ContratoId);

            modelBuilder.Entity<MantenimientoDetalle>()
                .HasOne(x => x.Mantenimiento)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.MantenimientoId);

            modelBuilder.Entity<InspeccionDetalle>()
                .HasOne(x => x.Inspeccion)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.InspeccionId);

            modelBuilder.Entity<PagoDetalle>()
                .HasOne(x => x.Pago)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.PagoId);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) ||
                        property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }
                }

                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }
    }
}