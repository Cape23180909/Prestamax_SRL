using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Models;

namespace Prestamax_SRL.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<Prestamos> Prestamos { get; set; }
    public DbSet<Cobros> Cobros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Asegúrate de llamar a la base

        modelBuilder.Entity<Cobros>()
            .HasKey(c => c.CobroId);

        modelBuilder.Entity<Cobros>()
            .HasOne(c => c.Cliente)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict); // Cambia según sea necesario

        modelBuilder.Entity<Cobros>()
            .HasOne(c => c.Prestamo)
            .WithMany(p =>p.Cobros)
            .HasForeignKey(c => c.PrestamoId)
            .OnDelete(DeleteBehavior.Cascade); // Cambia según sea necesario
    }
}