using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Data;
using Prestamax_SRL.Models;
using System.Linq.Expressions;

namespace Prestamax_SRL.Services;
public class PrestamoService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory; // Cambiado a IDbContextFactory

    // Constructor que inicializa _dbFactory
    public PrestamoService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory)); // Inicializa el campo
    }

    // Metodo Existe 
    public async Task<bool> Existe(int prestamoid)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        return await contexto.Prestamos.AnyAsync(p => p.PrestamosId == prestamoid);
    }

    // Metodo Insertar
    private async Task<bool> Insertar(Prestamos prestamo)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        contexto.Prestamos.Add(prestamo);
        return await contexto.SaveChangesAsync() > 0;
    }

    // Metodo Modificar 
    private async Task<bool> Modificar(Prestamos prestamo)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        contexto.Update(prestamo);
        return await contexto.SaveChangesAsync() > 0;
    }

    // Metodo Guardar 
    public async Task<bool> Guardar(Prestamos prestamo)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        if (!await Existe(prestamo.PrestamosId))
            return await Insertar(prestamo);
        else
            return await Modificar(prestamo);
    }

    // Metodo Eliminar
    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        var eliminado = await contexto.Prestamos
            .Where(p => p.PrestamosId == id)
            .ExecuteDeleteAsync();
        return eliminado > 0;
    }

    // Metodo Buscar 
    public async Task<Prestamos?> Buscar(int id)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        return await contexto.Prestamos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PrestamosId == id);
    }

    // Metodo Listar 
    public async Task<List<Prestamos>> Listar(Expression<Func<Prestamos, bool>> criterio)
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        return await contexto.Prestamos
            .AsNoTracking()
            .Where(criterio)
            .ToListAsync();
    }

    public async Task<List<Prestamos>> ListarPrestamosConClientes()
    {
        await using var contexto = await _dbFactory.CreateDbContextAsync(); // Usa el DbFactory
        return await contexto.Prestamos
            .Include(p => p.Cliente)
            .ToListAsync();
    }
}